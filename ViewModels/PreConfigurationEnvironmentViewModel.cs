// ViewModels/PreConfigurationEnvironmentViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using OpenessApp.Models;
using OpenessApp.Properties;

namespace OpenessApp.ViewModels
{
    public class PreConfigurationEnvironmentViewModel : BindableBase
    {
        // 1️⃣ Der bekannte Siemens-Engineering-DLL-Namen
        private readonly string[] _knownModules = new[]
        {
            "Step7.Engineering.dll",
            "WinCC.Engineering.dll",
            "Startdrive.Engineering.dll",
            "WinCCUnified.Engineering.dll",
            // …weitere Module hier ergänzen…
        };

        // 2️⃣ Der Pfad, in dem wir suchen (Standard aus Settings)
        private string _modulePath = Settings.Default.SelectedModulePath
                                     ?? @"C:\ProgramData\Siemens\TIA\Modules";
        public string ModulePath
        {
            get => _modulePath;
            set
            {
                if (SetProperty(ref _modulePath, value))
                    LoadModulesStatic();  // bei Änderung neu laden
            }
        }

        // 3️⃣ Sammlung der gefundenen Module (niemals null)
        public ObservableCollection<ModuleOption> Modules { get; }
            = new ObservableCollection<ModuleOption>();

        // 4️⃣ Commands
        public DelegateCommand ConfirmationCommand { get; }
        public DelegateCommand BrowsePathCommand { get; }

        // 5️⃣ DialogResult für das Schließen des Fensters
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public PreConfigurationEnvironmentViewModel()
        {
            // Module sofort beim Start laden
            LoadModulesStatic();

            // Commands belegen
            ConfirmationCommand = new DelegateCommand(OnConfirm);
            BrowsePathCommand = new DelegateCommand(OnBrowsePath);
        }

        /// <summary>
        /// Variante 1: Prüft im Verzeichnis, welche der _knownModules existieren
        /// </summary>
        private void LoadModulesStatic()
        {
            Modules.Clear();

            if (!Directory.Exists(ModulePath))
                return;

            foreach (var dll in _knownModules)
            {
                var fullPath = Path.Combine(ModulePath, dll);
                if (File.Exists(fullPath))
                {
                    Modules.Add(new ModuleOption(fullPath)
                    {
                        IsSelected = true  // per default vorausgewählt
                    });
                }
            }

            // Falls der Anwender bereits gespeichert hatte, Auswahl nachladen
            var saved = Settings.Default.SelectedModules;
            if (!string.IsNullOrWhiteSpace(saved))
            {
                var sel = saved.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var m in Modules)
                    m.IsSelected = sel.Contains(m.AssemblyName);
            }
        }

        private void OnConfirm()
        {
            // Speichern
            Settings.Default.SelectedModulePath = ModulePath;
            var chosen = Modules.Where(m => m.IsSelected)
                                .Select(m => m.AssemblyName);
            Settings.Default.SelectedModules = string.Join(";", chosen);
            Settings.Default.Save();

            DialogResult = true;
        }

        private void OnBrowsePath()
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Modulverzeichnis auswählen",
                SelectedPath = ModulePath
            })
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    ModulePath = dlg.SelectedPath;
            }
        }
    }
}
