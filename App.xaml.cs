using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Prism.Regions;
using OpenessApp.Views;
using OpenessApp.ViewModels;
using OpenessApp.Adapter;
using System.Windows.Controls;
using System.Linq;
using System.Reflection;
using System.IO;

namespace OpenessApp
{
    public partial class App : PrismApplication
    {
        // Wird nach App.Start() automatisch von Prism aufgerufen
        protected override void OnInitialized()
        {
            // 1.) Version-Auswahl (bestehender Code)…
            var versionDialog = Container.Resolve<PreSelectionAssemblyVersionView>();
            if (versionDialog.ShowDialog() != true)
            {
                Shutdown();
                return;
            }

            // 2.) Jetzt PreConfigurationEnvironment öffnen
            var preEnvDialog = Container.Resolve<PreConfigurationEnvironmentView>();
            var preEnvVm = (PreConfigurationEnvironmentViewModel)preEnvDialog.DataContext;
            if (preEnvDialog.ShowDialog() != true)
            {
                // Benutzer hat abgebrochen
                Shutdown();
                return;
            }

            // 3.) Ausgewählte Module laden
            var moduleProvider = Container.Resolve<IModuleProvider>();
            foreach (var opt in preEnvVm.Modules.Where(m => m.IsSelected))
            {
                // DLL-Pfad zusammenbauen
                var dllPath = Path.Combine(preEnvVm.ModulePath, opt.EngineeringDll);
                var asm = Assembly.LoadFrom(dllPath);
                moduleProvider.AddModule(asm);
            }

            // 4.) Service konfigurieren
            moduleProvider.ConfigureServices(
                preEnvVm.SelectedEngineeringVersion,
                preEnvVm.SelectedApiVersion);

            // 5.) Shell anzeigen
            base.OnInitialized();
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
