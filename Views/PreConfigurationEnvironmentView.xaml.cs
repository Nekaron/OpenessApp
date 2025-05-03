using System.Windows;
using OpenessApp.ViewModels;

namespace OpenessApp.Views
{
    public partial class PreConfigurationEnvironmentView : Window
    {
        public PreConfigurationEnvironmentView()
        {
            InitializeComponent();
            Loaded += PreConfigurationEnvironmentView_Loaded;
        }

        private void PreConfigurationEnvironmentView_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as PreConfigurationEnvironmentViewModel;
            if (vm != null)
                vm.OnLoaded();
        }
    }
}
