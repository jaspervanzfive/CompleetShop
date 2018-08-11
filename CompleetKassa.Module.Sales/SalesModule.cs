using CompleetKassa.Common;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace CompleetKassa.Modules.Sales
{
	public class SalesModule : IModule
	{
		private IRegionManager _regionManager;
		private IUnityContainer _container;

		public SalesModule(IUnityContainer container, IRegionManager regionManager)
		{
			_container = container;
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Views.Sales));
		}
	}
}