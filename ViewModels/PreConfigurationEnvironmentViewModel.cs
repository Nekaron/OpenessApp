using OpenessApp.Models;
using OpenessApp.Properties;
using Prism.Mvvm;
using Prism.Commands;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.IO;
namespace OpenessApp.ViewModels
{
    public class PreConfigurationEnvironmentViewModel : BindableBase
    {
        // 1. Modulverzeichnis (wird gespeichert)
        private string _modulePath;

        public string ModulePath
        {
            get => _modulePath;
            set => SetProperty(ref _modulePath, value);
        }

        // 2. Liste der Module
        public ObservableCollection<TiaPortalModulesAndOptions> ModulesAndOptions { get; }

        // 3. Commands
        public DelegateCommand ConfirmationCommand { get; }
        public DelegateCommand GetPathFromModulesAndOptionsCommand { get; }

        // 4. Rückgabe für Dialogfenster
        private bool? _dialogResult;

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        // 5. Konstruktor
        public PreConfigurationEnvironmentViewModel()
        {
            ModulesAndOptions = new ObservableCollection<TiaPortalModulesAndOptions>();

            LoadModules(); // 🔁 beim Start alle Module laden

            ConfirmationCommand = new DelegateCommand(OnConfirm);
            GetPathFromModulesAndOptionsCommand = new DelegateCommand(OnSelectPath);
        }
        private void LoadModules()
        {
            ModulesAndOptions.Clear();

            if (!Directory.Exists(ModulePath))
            {
                Trace.WriteLine($"Pfad existiert nicht: {ModulePath}");
                return;
            }

            var dllFiles = Directory
                .EnumerateFiles(ModulePath, "*.dll", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f).Contains("Engineering"));

            foreach (var dll in dllFiles)
            {
                Trace.WriteLine($"Gefunden: {dll}");
                var fileName = Path.GetFileName(dll);

                ModulesAndOptions.Add(new TiaPortalModulesAndOptions
                {
                    AssemblyName = Path.GetFileNameWithoutExtension(fileName),
                    EngineeringDll = fileName,
                    VersionInfo = "V19",
                    IsSelected = false
                });
            }

            var saved = Settings.Default.SelectedModules;
            if (!string.IsNullOrEmpty(saved))
            {
                var selected = saved.Split(';');
                foreach (var mod in ModulesAndOptions)
                    mod.IsSelected = selected.Contains(mod.AssemblyName);
            }
        }



        // Wird beim Klick auf "Confirm" ausgeführt
        private void OnConfirm()
        {
            // Auswahl speichern
            Settings.Default.SelectedModulePath = ModulePath;

            var selected = ModulesAndOptions
                .Where(m => m.IsSelected)
                .Select(m => m.AssemblyName);

            Settings.Default.SelectedModules = string.Join(";", selected);
            Settings.Default.Save();

            DialogResult = true; // Fenster schließen
        }

        // Öffnet Verzeichnisauswahl (kompatibel zu .NET Framework)
        private void OnSelectPath()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Modulverzeichnis auswählen",
                SelectedPath = ModulePath
            };

            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ModulePath = dialog.SelectedPath;
            }

            dialog.Dispose(); // wichtig für C# 7.3
        }
    }
}
