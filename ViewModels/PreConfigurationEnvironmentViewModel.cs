// ViewModels/PreConfigurationEnvironmentViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Prism.Mvvm;
using Prism.Commands;
using OpenessApp.Models;
using OpenessApp.Properties;

namespace OpenessApp.ViewModels
{
    public class PreConfigurationEnvironmentViewModel : BindableBase
    {
        // 1️⃣ Modulpfad (Standard aus Settings oder Fallback)
        private string _modulePath = Settings.Default.SelectedModulePath
                                     ?? @"c:\Program Files\Siemens\Automation\Portal V19\PublicAPI\V19";
        public string ModulePath
        {
            get => _modulePath;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !Directory.Exists(value))
                {
                    Debug.WriteLine($"[ModulePath] Ungültiger Pfad: {value}");
                    return;
                }

                if (SetProperty(ref _modulePath, value))
                {
                    LoadModules(); // Neu laden, wenn der Pfad gültig ist
                }
            }
        }

        // 2️⃣ Sammlung aller gefundenen Module (niemals null)
        public ObservableCollection<TiaPortalModulesAndOptions> ModulesAndOptions { get; }
            = new ObservableCollection<TiaPortalModulesAndOptions>();

        // 3️⃣ Commands für UI-Buttons
        public DelegateCommand ConfirmationCommand { get; }
        public DelegateCommand GetPathFromModulesAndOptionsCommand { get; }

        // 4️⃣ DialogResult für das Fenster-Close-Behavior
        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        // Konstruktor
        public PreConfigurationEnvironmentViewModel()
        {
            if (!Directory.Exists(ModulePath))
            {
                Debug.WriteLine($"[Konstruktor] Standardpfad ungültig: {ModulePath}");
                ModulePath = @"C:\Fallback\Pfad"; // Alternativer Fallback
            }

            LoadModules();
            ConfirmationCommand = new DelegateCommand(OnConfirm);
            GetPathFromModulesAndOptionsCommand = new DelegateCommand(OnSelectPath);
        }

        // Lädt alle *.dll und filtert "Engineering"
        private void LoadModules()
        {
            if (string.IsNullOrWhiteSpace(ModulePath))
            {
                Debug.WriteLine("[LoadModules] ModulePath ist leer oder null.");
                return;
            }

            if (!Directory.Exists(ModulePath))
            {
                Debug.WriteLine($"[LoadModules] Pfad existiert nicht: {ModulePath}");
                return;
            }

            ModulesAndOptions.Clear();
            Debug.WriteLine($"[LoadModules] Suche in: {ModulePath}");

            // Restlicher Code...
        }

        // Wird beim Klick auf "Confirm" ausgeführt
        private void OnConfirm()
        {
            // Pfad + Auswahl in Settings speichern
            Settings.Default.SelectedModulePath = ModulePath;
            var chosen = ModulesAndOptions
                .Where(m => m.IsSelected)
                .Select(m => m.AssemblyName);
            Settings.Default.SelectedModules = string.Join(";", chosen);
            Settings.Default.Save();

            // Dialog schließen
            DialogResult = true;
        }

        // Öffnet Verzeichnis-Dialog (WinForms) und lädt danach neu
        private void OnSelectPath()
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Modulverzeichnis auswählen",
                SelectedPath = ModulePath
            })
            {
                var result = dlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    ModulePath = dlg.SelectedPath;
                }
            }
        }
    }
}
