using System.Windows;
using OpenessApp.ViewModels;

namespace OpenessApp.Views
{
    public partial class PreConfigurationEnvironmentView : Window
    {
        public PreConfigurationEnvironmentView()
        {
            InitializeComponent();

            if (DataContext is PreConfigurationEnvironmentViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(vm.DialogResult) && vm.DialogResult == true)
                    {
                        DialogResult = true;
                        Close();
                    }
                };
            }
        }
    }
}
