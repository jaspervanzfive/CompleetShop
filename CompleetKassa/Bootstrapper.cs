using System.Windows;
using System.Windows.Controls;
using CompleetKassa.Module.Customer;
using CompleetKassa.Modules.Products;
using CompleetKassa.Modules.Sales;
using CompleetKassa.RegionAdapters;
using CompleetKassa.Views;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;

namespace CompleetKassa
{
	public class Bootstrapper : UnityBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			return Container.TryResolve<MainWindow>();
		}

		protected override void InitializeShell()
		{
			Application.Current.MainWindow.Show();
		}

		protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
		{
			RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();

			mappings.RegisterMapping(typeof(StackPanel), Container.TryResolve<StackPanelRegionAdapter>());

			return mappings;
		}

		protected override void ConfigureModuleCatalog()
		{
			var catalog = (ModuleCatalog)ModuleCatalog;


			catalog.AddModule(typeof(SalesModule));
			catalog.AddModule(typeof(ProductsModule));
			catalog.AddModule(typeof(CustomersModule));
		}
	}
}
