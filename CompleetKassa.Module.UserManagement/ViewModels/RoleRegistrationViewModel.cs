using System;
using CompleetKassa.DataValidation;
using CompleetKassa.Module.UserManagement.Commands;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Events;
using Prism.Regions;

namespace CompleetKassa.Module.UserManagement.ViewModels
{
    public class RoleRegistrationViewModel : ViewModelValidationBase, IActiveAware
	{
		#region Fields
		private IEventAggregator _eventAggregator;
		private IRegionManager _regionManager;
		IModuleCommands _moduleCommands;
		#endregion Fields

		#region Bindable Property
		private string _title;
		public string Title
		{
			get { return _title; }
			set { SetProperty (ref _title, value); }
		}

		private bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnIsActiveChanged ();
			}
		}
		#endregion Bindable Property

		private void OnIsActiveChanged ()
		{
			//OnAddCommand.IsActive = IsActive;

			IsActiveChanged?.Invoke (this, new EventArgs ());
		}

		public event EventHandler IsActiveChanged;

		#region Constructor
		public RoleRegistrationViewModel (IUnityContainer container)
		{
			_regionManager = container.Resolve<IRegionManager> ();
			_eventAggregator = container.Resolve<IEventAggregator> ();
			_moduleCommands = container.Resolve<IModuleCommands> ();

			Title = "Role";
		}
		#endregion Constructor
	}
}