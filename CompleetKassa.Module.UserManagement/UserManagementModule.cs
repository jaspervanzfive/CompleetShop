using CompleetKassa.Definitions;
using CompleetKassa.Module.UserManagement.Commands;
using CompleetKassa.Module.UserManagement.Definitions;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CompleetKassa.Module.UserManagement
{
    public class UserManagementModule : IModule
    {
        private IRegionManager _regionManager;
        private IUnityContainer _container;

        public UserManagementModule(IUnityContainer container)
        {
            _container = container;
            _regionManager = container.Resolve<IRegionManager>();

            container.RegisterType<IModuleCommands, ModuleCommands>(new ContainerControlledLifetimeManager());
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MenuBarRegion, () => _container.Resolve<Views.UserManagementMenu>());
            _regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, () => _container.Resolve<Views.UserManagement>());

            IRegion region = _regionManager.Regions[ModuleRegionNames.UserModuleContentRegion];
            region.Add(_container.Resolve<Views.UserRegistration>());
            region.Add(_container.Resolve<Views.RoleRegistration>());
            region.Add(_container.Resolve<Views.ResourceRegistration>());
        }
    }
}
