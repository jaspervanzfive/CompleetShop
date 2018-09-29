using CompleetKassa.Definitions;
using CompleetKassa.Module.ProductManagement.Definitions;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CompleetKassa.Module.ProductManagement
{
    public class ProductManagementModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public ProductManagementModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MenuBarRegion, () => _container.Resolve<Views.ProductManagementMenu>());
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, () => _container.Resolve<Views.ProductManagement>());

            IRegion region = _regionManager.Regions[ModuleRegionNames.ProductModuleContentRegion];
            region.Add(_container.Resolve<Views.ProductRegistration>());
            region.Add(_container.Resolve<Views.CategoryRegistration>());
        }
    }
}