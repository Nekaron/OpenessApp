using OpenessApp.Models;
using OpenessApp.Properties;
using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows.Forms; // Für FolderBrowserDialog

namespace OpenessApp.ViewModels
{
    public class PreConfigurationEnvironmentViewModel : BindableBase
    {
        // 1) Speichert den aktuellen Modulpfad
        private string _modulePath = Settings.Default.SelectedModulePath;
        public string ModulePath
        {
            get => _modulePath;
            set
            {
                if (SetProperty(ref _modulePath, value))
                    LoadModules();
            }
        }

        // 2) Liste aller Module im aktuellen Pfad
        public ObservableCollection<ModuleOption> Modules { get; }

        // 3) Commands für UI
        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand BrowsePathCommand { get; }

        // 4) DialogResult zum Schließen der View
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public PreConfigurationEnvironmentViewModel()
        {
            Modules = new ObservableCollection<ModuleOption>();
            ConfirmCommand = new DelegateCommand(OnConfirm);
            BrowsePathCommand = new DelegateCommand(OnBrowse);
            LoadModules(); // initial befüllen
        }

        private void LoadModules()
        {
            Modules.Clear();
            if (!Directory.Exists(ModulePath))
                return;

            var dlls = Directory.EnumerateFiles(ModulePath, "*.Engineering.dll", SearchOption.AllDirectories);
            foreach (var path in dlls)
            {
                var file = Path.GetFileName(path);
                var name = Path.GetFileNameWithoutExtension(file);
                Modules.Add(new ModuleOption
                {
                    AssemblyName = name,
                    EngineeringDll = file,
                    VersionInfo = "V19",
                    IsSelected = Settings.Default.SelectedModules?.Split(';').Contains(name) == true
                });
            }
        }

        private void OnConfirm()
        {
            // Speichern
            Settings.Default.SelectedModulePath = ModulePath;
            var selected = Modules.Where(m => m.IsSelected).Select(m => m.AssemblyName);
            Settings.Default.SelectedModules = string.Join(";", selected);
            Settings.Default.Save();

            DialogResult = true;
        }

        private void OnBrowse()
        {
            using (var dlg = new FolderBrowserDialog
            {
                Description = "Modulverzeichnis auswählen",
                SelectedPath = ModulePath
            })
            {
                if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ModulePath = dlg.SelectedPath;
                }
            }
        }
    }
}
