using System.Windows;
using OpenessApp.ViewModels;

namespace OpenessApp.Views
{
    public partial class PreSelectionAssemblyVersionView : Window
    {
        public PreSelectionAssemblyVersionView()
        {
            InitializeComponent();
            DataContext = new PreSelectionAssemblyVersionViewModel();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;  // Nur gültig bei ShowDialog()
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
