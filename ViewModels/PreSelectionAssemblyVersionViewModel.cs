using Prism.Mvvm;
using Prism.Commands;
using System.Collections.ObjectModel;
using System;

namespace OpenessApp.ViewModels
{
    public class PreSelectionAssemblyVersionViewModel : BindableBase
    {
        public ObservableCollection<string> EngineeringVersions { get; }
        public ObservableCollection<string> ApiVersions { get; }

        private string _selectedEngineeringVersion;
        public string SelectedEngineeringVersion
        {
            get { return _selectedEngineeringVersion; }
            set { SetProperty(ref _selectedEngineeringVersion, value); }
        }

        private string _selectedApiVersion;
        public string SelectedApiVersion
        {
            get { return _selectedApiVersion; }
            set { SetProperty(ref _selectedApiVersion, value); }
        }

        public DelegateCommand ConfirmCommand { get; }

        public event Action RequestClose;

        public PreSelectionAssemblyVersionViewModel()
        {
            EngineeringVersions = new ObservableCollection<string> { "V17", "V18", "V19" };
            ApiVersions = new ObservableCollection<string> { "Openess 17", "Openess 18", "Openess 19" };

            // V19 als Vorauswahl
            SelectedEngineeringVersion = "V19";
            SelectedApiVersion = "Openess 19";

            ConfirmCommand = new DelegateCommand(OnConfirm);
        }

        private void OnConfirm()
        {
            // Hier nur das Schließen anstoßen – View entscheidet, was passiert
            RequestClose?.Invoke();
        }
    }
}
