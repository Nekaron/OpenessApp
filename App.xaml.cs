using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Prism.Regions;
using OpenessApp.Views;
using OpenessApp.ViewModels;
using OpenessApp.Adapter;
using System.Windows.Controls;

namespace OpenessApp
{
    public partial class App : PrismApplication
    {
        // Wird nach App.Start() automatisch von Prism aufgerufen
        protected override void OnInitialized()
        {
            // 1️⃣ Auswahl der TIA- und API-Version
            var tiaDialog = new PreSelectionAssemblyVersionView();
            if (tiaDialog.ShowDialog() != true)
            {
                Shutdown(); // Benutzer hat abgebrochen
                return;
            }

            var tiaVm = tiaDialog.DataContext as PreSelectionAssemblyVersionViewModel;
            if (tiaVm != null)
            {
                string tia = tiaVm.SelectedEngineeringVersion;
                string api = tiaVm.SelectedApiVersion;
                MessageBox.Show("Gewählt: TIA " + tia + ", API " + api);
            }

            // 2️⃣ Modulauswahl anzeigen (als echtes Dialogfenster)
            var configDialog = new PreConfigurationEnvironmentView();
            if (configDialog.ShowDialog() != true)
            {
                Shutdown(); // Benutzer hat abgebrochen
                return;
            }

            // 3️⃣ Region-Manager laden
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("NavigationRegion", "NavigationView");
            regionManager.RequestNavigate("LogRegion", "TraceLogView");

            // 4️⃣ Log initialisieren
            var log = Container.Resolve<Services.TraceLogService>();
            log.Write("TIA geladen.");
            log.Write("StartView geöffnet.");
            log.Write("Dies ist eine Info-Meldung.", "INFO");
            log.Write("Das ist ein Fehler!", "ERROR");
            log.Write("Achtung, mögliche Warnung!", "WARN");

            base.OnInitialized(); // Wichtig: danach Shell anzeigen
        }

        // Prism erzeugt und zeigt dieses Fenster als Hauptfenster
        protected override Window CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        // Registrierung aller Views, ViewModels und Dienste
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Navigation: View → ViewModel (optional)
            containerRegistry.RegisterForNavigation<StartView>();
            containerRegistry.RegisterForNavigation<NavigationView, NavigationViewModel>();
            containerRegistry.RegisterForNavigation<TraceLogView, TraceLogViewModel>();

            // Globale Dienste
            containerRegistry.RegisterSingleton<Services.TraceLogService>();
            containerRegistry.RegisterSingleton<StackPanelRegionAdapter>();
        }

        // StackPanel als Region registrieren
        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings mappings)
        {
            base.ConfigureRegionAdapterMappings(mappings);

            mappings.RegisterMapping(typeof(StackPanel),
                Container.Resolve<StackPanelRegionAdapter>());
        }
    }
}
