using Prism.Mvvm;
using Prism.Regions;

namespace OpenessApp.ViewModels
{
    public class StartViewModel : BindableBase, INavigationAware
    {
        private string _tia;
        public string TiaVersion
        {
            get { return _tia; }
            set { SetProperty(ref _tia, value); }
        }

        private string _api;
        public string ApiVersion
        {
            get { return _api; }
            set { SetProperty(ref _api, value); }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            TiaVersion = (string)navigationContext.Parameters["tia"];
            ApiVersion = (string)navigationContext.Parameters["api"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
