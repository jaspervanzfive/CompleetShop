using CompleetKassa.Common;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CompleetKassa.Module.Customer
{
	public class CustomersModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public CustomersModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
			_regionManager.RegisterViewWithRegion(RegionNames.MenuBarRegion, typeof(Views.CustomersMenu));
			_regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.Customers));
        }
    }
}