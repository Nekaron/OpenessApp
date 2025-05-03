using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using OpenessApp.Services;

namespace OpenessApp.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly TraceLogService _log; // 💡 der zentrale Logservice

        public DelegateCommand NavigateCommand { get; }

        public NavigationViewModel(IRegionManager regionManager, TraceLogService log)
        {
            _regionManager = regionManager;
            _log = log;

            NavigateCommand = new DelegateCommand(OnNavigate);
        }

        private void OnNavigate()
        {
            _log.Write("Navigation wurde ausgelöst."); // ✅ Log-Eintrag

            _regionManager.RequestNavigate("ContentRegion", "StartView");
        }
    }
}
