﻿using CompleetKassa.Definitions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace CompleetKassa.Module.ProductManagement.ViewModels
{
	public class ProductManagementMenuViewModel : BindableBase
	{
		private readonly IRegionManager _regionManager;

		public DelegateCommand OnNavigate { get; private set; }

		public string Color { get; private set; }

		public string Name { get; private set; }

		public string ImagePath { get; private set; }

		public ProductManagementMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;

			OnNavigate = new DelegateCommand(Navigate);

			Color = "#FDAC94";
			Name = "Products";
			ImagePath = "../Images/product.png";
		}

		private void Navigate()
		{
			_regionManager.RequestNavigate(RegionNames.ContentRegion, typeof(Views.ProductManagement).ToString());
		}
	}
}
