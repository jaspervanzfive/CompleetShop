using CompleetKassa.Definitions;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CompleetKassa.Modules.Products
{
	public class ProductsModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public ProductsModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
			_regionManager.RegisterViewWithRegion(RegionNames.MenuBarRegion, typeof(Views.ProductsMenu));
			_regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.Products));
        }
    }
}