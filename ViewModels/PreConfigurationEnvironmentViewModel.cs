using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace OpenessApp.ViewModels
{
    /* -----------------------------------------------------------
       Wizard‑Schritte  (außerhalb des VM → XAML findet das Enum)
       ----------------------------------------------------------- */
    public enum PreConfigStep
    {
        SelectVersions,
        SelectModules
    }

    /* -----------------------------------------------------------
       ViewModel
       ----------------------------------------------------------- */
    public class PreConfigurationEnvironmentViewModel : BindableBase
    {
        /* -----  DI‑Services  ----- */
        private readonly ISettingsService _settings;
        private readonly ITiaResolverService _resolver;
        private readonly IEventAggregator _eventAgg;

        /* -----  Konstruktor  ----- */
        public PreConfigurationEnvironmentViewModel(
            ISettingsService settings,
            ITiaResolverService resolver,
            IEventAggregator eventAgg)
        {
            _settings = settings;
            _resolver = resolver;
            _eventAgg = eventAgg;

            ConfirmVersionCommand = new DelegateCommand(ExecuteConfirmVersion, CanConfirmVersion);
            ConfirmModulesCommand = new DelegateCommand(ExecuteConfirmModules, CanConfirmModules);
            BackCommand = new DelegateCommand(() => Step = PreConfigStep.SelectVersions);

            EngineeringVersions = new ObservableCollection<string>();
            OpennessVersions = new ObservableCollection<string>();
            Modules = new ObservableCollection<ModuleSelectable>();

            Modules.CollectionChanged += (s, e) => ConfirmModulesCommand.RaiseCanExecuteChanged();
        }

        /* -----  Schritt 1 – Versionen  ----- */
        public ObservableCollection<string> EngineeringVersions { get; }
        public ObservableCollection<string> OpennessVersions { get; }

        private string _selectedEngineeringVersion;
        public string SelectedEngineeringVersion
        {
            get { return _selectedEngineeringVersion; }
            set
            {
                if (SetProperty(ref _selectedEngineeringVersion, value))
                    ConfirmVersionCommand.RaiseCanExecuteChanged();
            }
        }

        private string _selectedApiVersion;   // entspricht „SelectedOpennessVersion“
        public string SelectedApiVersion
        {
            get { return _selectedApiVersion; }
            set
            {
                if (SetProperty(ref _selectedApiVersion, value))
                    ConfirmVersionCommand.RaiseCanExecuteChanged();
            }
        }

        /* -----  Schritt 2 – Module  ----- */
        public ObservableCollection<ModuleSelectable> Modules { get; }

        /* Pfad, in dem die Module‑DLLs liegen → wird in ExecuteConfirmVersion gesetzt */
        public string ModulePath { get; private set; }

        /* -----  Wizard‑Status  ----- */
        private PreConfigStep _step = PreConfigStep.SelectVersions;
        public PreConfigStep Step
        {
            get { return _step; }
            set { SetProperty(ref _step, value); }
        }

        /* -----  Commands  ----- */
        public DelegateCommand ConfirmVersionCommand { get; }
        public DelegateCommand ConfirmModulesCommand { get; }
        public DelegateCommand BackCommand { get; }

        /* =========================================================
           Aufruf aus dem View‑Loaded‑Event
           ========================================================= */
        public void OnLoaded()
        {
            EngineeringVersions.Clear();
            OpennessVersions.Clear();

            foreach (var v in _settings.GetAvailableEngineeringVersions())
                EngineeringVersions.Add(v);

            foreach (var v in _settings.GetAvailableOpennessVersions())
                OpennessVersions.Add(v);

            Step = PreConfigStep.SelectVersions;
        }

        /* =========================================================
           Commands
           ========================================================= */
        private bool CanConfirmVersion()
        {
            return !string.IsNullOrEmpty(SelectedEngineeringVersion)
                && !string.IsNullOrEmpty(SelectedApiVersion);
        }

        private void ExecuteConfirmVersion()
        {
            /* 1 – Assemblies vorbereiten */
            _resolver.Load(SelectedEngineeringVersion, SelectedApiVersion);

            /* 2 – Module einlesen */
            ModulePath = _settings.GetModulesPath(SelectedEngineeringVersion);
            LoadModules(ModulePath);

            Step = PreConfigStep.SelectModules;
            ConfirmModulesCommand.RaiseCanExecuteChanged();
        }

        private bool CanConfirmModules()
        {
            return Modules.Any(m => m.IsSelected);
        }

        private void ExecuteConfirmModules()
        {
            var chosen = Modules.Where(m => m.IsSelected).ToList();

            _settings.SaveSelectedModules(chosen.Select(c => c.Name));
            _eventAgg.GetEvent<ModulesChosenEvent>().Publish(chosen);
            _eventAgg.GetEvent<PreConfigFinishedEvent>().Publish();
        }

        /* =========================================================
           Helpers
           ========================================================= */
        private void LoadModules(string root)
        {
            Modules.Clear();
            if (!Directory.Exists(root)) return;

            foreach (var dll in Directory.EnumerateFiles(root, "*Module.dll"))
            {
                var info = FileVersionInfo.GetVersionInfo(dll);
                var mod = new ModuleSelectable
                {
                    Name = Path.GetFileNameWithoutExtension(dll),
                    Version = info.FileVersion ?? "—",
                    IsSelected = true,
                    EngineeringDll = Path.GetFileName(dll)   // wichtig für App.OnInitialized()
                };

                mod.PropertyChanged += Module_PropertyChanged;
                Modules.Add(mod);
            }
        }

        private void Module_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ModuleSelectable.IsSelected))
                ConfirmModulesCommand.RaiseCanExecuteChanged();
        }

        /* =========================================================
           DTO – ModuleSelectable  (jetzt mit EngineeringDll)
           ========================================================= */
        public class ModuleSelectable : BindableBase
        {
            private string _name;
            public string Name
            {
                get { return _name; }
                set { SetProperty(ref _name, value); }
            }

            private string _version;
            public string Version
            {
                get { return _version; }
                set { SetProperty(ref _version, value); }
            }

            private bool _isSelected;
            public bool IsSelected
            {
                get { return _isSelected; }
                set { SetProperty(ref _isSelected, value); }
            }

            /* Dateiname der DLL  (z.B. „MyModule.Engineering.dll“) */
            public string EngineeringDll { get; set; }
        }
    }

    /* =============================================================
       Prism‑Events  (löschen, falls schon vorhanden)
       ============================================================= */
    public class ModulesChosenEvent : PubSubEvent<IList<PreConfigurationEnvironmentViewModel.ModuleSelectable>> { }
    public class PreConfigFinishedEvent : PubSubEvent { }

    /* =============================================================
       STUB‑Services & Provider  (durch reale impl. ersetzen)
       ============================================================= */
    public interface ISettingsService
    {
        IEnumerable<string> GetAvailableEngineeringVersions();
        IEnumerable<string> GetAvailableOpennessVersions();
        string GetModulesPath(string engineeringVersion);
        void SaveSelectedModules(IEnumerable<string> moduleNames);
    }

    public interface ITiaResolverService
    {
        void Load(string engineeringVersion, string apiVersion);
    }

    /* Damit dein App‑Code kompiliert */
    public interface IModuleProvider
    {
        void AddModule(System.Reflection.Assembly asm);
        void ConfigureServices(string engineeringVersion, string apiVersion);
    }
}
