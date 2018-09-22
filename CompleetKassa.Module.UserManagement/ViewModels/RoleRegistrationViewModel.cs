using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompleetKassa.DataValidation;
using CompleetKassa.Module.UserManagement.Commands;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Events;
using Prism.Regions;
using Prism.Commands;
using CompleetKassa.Models;
using CompleetKassa.Database.Services;
using System.Windows.Data;
using System.ComponentModel;
using static CompleetKassa.Module.UserManagement.Definitions.Enumeration;
using System.Windows.Media;

namespace CompleetKassa.Module.UserManagement.ViewModels
{
	public class RoleRegistrationViewModel : ViewModelValidationBase, IActiveAware
	{
		#region Fields
		private IRoleService _roleService;
		private IResourceService _resourceService;
		private IEventAggregator _eventAggregator;
		private IRegionManager _regionManager;
		IModuleCommands _moduleCommands;

		private int _roleListViewCount;
		private ExecutionTypes _currentExecutionType;
		#endregion Fields

		#region Property
		public Task Initialization { get; private set; }
		#endregion Property

		#region Enable/disable Command Property
		private bool _canAddRoleInfo;
		public bool CanAddRoleInfo
		{
			get { return _canAddRoleInfo; }
			set { SetProperty (ref _canAddRoleInfo, value); }
		}

		private bool _canEditRole;
		public bool CanEditRole
		{
			get { return _canEditRole; }
			set { SetProperty (ref _canEditRole, value); }
		}

		private bool _canDeleteRole;
		public bool CanDeleteRole
		{
			get { return _canDeleteRole; }
			set { SetProperty (ref _canDeleteRole, value); }
		}

		private bool _canSaveRole;
		public bool CanSaveRole
		{
			get { return _canSaveRole; }
			set { SetProperty (ref _canSaveRole, value); }
		}

		private bool _canCancelRole;
		public bool CanCancelRole
		{
			get { return _canCancelRole; }
			set { SetProperty (ref _canCancelRole, value); }
		}

		private bool _canNavToFirst;
		public bool CanNavToFirst
		{
			get { return _canNavToFirst; }
			set { SetProperty (ref _canNavToFirst, value); }
		}

		private bool _canNavToLast;
		public bool CanNavToLast
		{
			get { return _canNavToLast; }
			set { SetProperty (ref _canNavToLast, value); }
		}

		private bool _canNavToPrevious;
		public bool CanNavToPrevious
		{
			get { return _canNavToPrevious; }
			set { SetProperty (ref _canNavToPrevious, value); }
		}

		private bool _canNavToNext;
		public bool CanNavToNext
		{
			get { return _canNavToNext; }
			set { SetProperty (ref _canNavToNext, value); }
		}
		#endregion Enable/disable Command Property

		#region Command Property
		public DelegateCommand OnAddCommand { get; private set; }
		public DelegateCommand OnEditCommand { get; private set; }
		public DelegateCommand OnSaveCommand { get; private set; }
		public DelegateCommand OnDeleteCommand { get; private set; }
		public DelegateCommand OnCancelCommand { get; private set; }
		public DelegateCommand<RoleModel> OnDeleteRoleCommand { get; private set; }

		public DelegateCommand OnFirstNavCommand { get; private set; }
		public DelegateCommand OnPreviousNavCommand { get; private set; }
		public DelegateCommand OnNextNavCommand { get; private set; }
		public DelegateCommand OnLastNavCommand { get; private set; }

		public DelegateCommand OnValidationErrorCommand { get; private set; }
		#endregion Command Property

		#region Bindable Property
		private int _roleID;
		public int RoleID
		{
			get { return _roleID; }
			set
			{
				SetProperty (ref _roleID, value);

			}
		}

		private string _roleName;
		public string RoleName
		{
			get { return _roleName; }
			set
			{
				SetProperty (ref _roleName, value);
			}
		}

		private string _roleDescription;
		public string RoleDescription
		{
			get { return _roleDescription; }
			set { SetProperty (ref _roleDescription, value); }
		}

		private ICollectionView _roleListView;
		public ICollectionView RoleListView
		{
			get { return _roleListView; }
			private set
			{

				SetProperty (ref _roleListView, value);

				if (_roleListView != null) {
					_roleListViewCount = _roleListView.Cast<object> ().Count ();
				}
			}
		}

		private ICollectionView _resourceListView;
		public ICollectionView ResourceListView
		{
			get { return _resourceListView; }
			private set
			{
				SetProperty (ref _resourceListView, value);
			}
		}

		private int _selectedRoleIndex;
		public int SelectedRoleIndex
		{
			get { return _selectedRoleIndex; }
			set
			{
				SetProperty (ref _selectedRoleIndex, value);
				SetCommandEnableStatus (ExecutionTypes.None);
				SetNavigationCommandEnableStatus ();
			}
		}

		private RoleModel _selectedRole;
		public RoleModel SelectedRole
		{
			get { return _selectedRole; }
			set
			{
				SetProperty (ref _selectedRole, value);

				RoleID = _selectedRole.ID;
				RoleName = _selectedRole.Name;
				RoleDescription = _selectedRole.Description;

				if (_selectedRole.Resources != null) {
					ResourceListView = new CollectionView (_selectedRole.Resources);
				}
			}
		}

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

		private bool _roleInfoFormVisibility;
		public bool RoleInfoFormVisibility
		{
			get { return _roleInfoFormVisibility; }
			set
			{
				SetProperty (ref _roleInfoFormVisibility, value);
			}
		}

		private bool _isRoleListEnabled;
		public bool IsRoleListEnabled
		{
			get { return _isRoleListEnabled; }
			set
			{
				SetProperty (ref _isRoleListEnabled, value);
			}
		}

		private bool _isRoleFormReadOnly;
		public bool IsRoleFormReadOnly
		{
			get { return _isRoleFormReadOnly; }
			set
			{
				SetProperty (ref _isRoleFormReadOnly, value);
				RoleFormColor = _isRoleFormReadOnly == true ? new SolidColorBrush (Colors.LightBlue) : new SolidColorBrush (Colors.CornflowerBlue);
			}
		}

		private Brush _roleFormColor;
		public Brush RoleFormColor
		{
			get { return _roleFormColor; }
			set
			{
				SetProperty (ref _roleFormColor, value);
			}
		}
		#endregion Bindable Property

		public event EventHandler IsActiveChanged;

		#region Constructor
		public RoleRegistrationViewModel (IUnityContainer container)
		{
			_regionManager = container.Resolve<IRegionManager> ();
			_roleService = container.Resolve<IRoleService> ();
			_resourceService = container.Resolve<IResourceService> ();
			_eventAggregator = container.Resolve<IEventAggregator> ();
			_moduleCommands = container.Resolve<IModuleCommands> ();

			InitializeFields ();
			InitializeCommandHandler ();
			SetCommandEnableStatus ();

			Initialization = InitializeAsync ();

			Title = "Role";
		}
		#endregion Constructor

		#region Methods
		private void InitializeFields ()
		{
			_roleListViewCount = 0;
			_currentExecutionType = ExecutionTypes.None;
		}
		private void InitializeCommandHandler ()
		{
			// Role operations
			OnAddCommand = new DelegateCommand (AddCommandHandler).ObservesCanExecute (() => CanAddRoleInfo);
			OnEditCommand = new DelegateCommand (EditCommandHandler).ObservesCanExecute (() => CanEditRole);
			OnDeleteCommand = new DelegateCommand (DeleteCommandHandler).ObservesCanExecute (() => CanDeleteRole);
			OnSaveCommand = new DelegateCommand (SaveCommandHandler).ObservesCanExecute (() => CanSaveRole);
			OnCancelCommand = new DelegateCommand (CancelCommandHandler).ObservesCanExecute (() => CanCancelRole);
			OnDeleteRoleCommand = new DelegateCommand<RoleModel> (DeleteRoleCommandHandler).ObservesCanExecute (() => CanDeleteRole);

			// Navigation
			OnFirstNavCommand = new DelegateCommand (FirstNavCommandHandler).ObservesCanExecute (() => CanNavToFirst);
			OnPreviousNavCommand = new DelegateCommand (PreviousNavCommandHandler).ObservesCanExecute (() => CanNavToPrevious);
			OnNextNavCommand = new DelegateCommand (NextNavCommandHandler).ObservesCanExecute (() => CanNavToNext);
			OnLastNavCommand = new DelegateCommand (LastNavCommandHandler).ObservesCanExecute (() => CanNavToLast);

			OnValidationErrorCommand = new DelegateCommand (ValidationErrorCommandHandler);

			_moduleCommands.NewCommand.RegisterCommand (OnAddCommand);
			_moduleCommands.EditCommand.RegisterCommand (OnEditCommand);
			_moduleCommands.DeleteCommand.RegisterCommand (OnDeleteCommand);
			_moduleCommands.SaveCommand.RegisterCommand (OnSaveCommand);
			_moduleCommands.CancelCommand.RegisterCommand (OnCancelCommand);

			_moduleCommands.FirstNavCommand.RegisterCommand (OnFirstNavCommand);
			_moduleCommands.PreviousNavCommand.RegisterCommand (OnPreviousNavCommand);
			_moduleCommands.NextNavCommand.RegisterCommand (OnNextNavCommand);
			_moduleCommands.LastNavCommand.RegisterCommand (OnLastNavCommand);

		}

		private async Task InitializeAsync ()
		{
			var result = await _roleService.GetRolesAsync ();
			if (result.DidError == false) {
				RoleListView = CollectionViewSource.GetDefaultView (result.Model.ToList ());
			}
		}

		#region Command Handler
		private void OnIsActiveChanged ()
		{
			OnCancelCommand.IsActive = IsActive;
			OnAddCommand.IsActive = IsActive;
			OnEditCommand.IsActive = IsActive;
			OnDeleteCommand.IsActive = IsActive;
			OnSaveCommand.IsActive = IsActive;

			OnFirstNavCommand.IsActive = IsActive;
			OnPreviousNavCommand.IsActive = IsActive;
			OnNextNavCommand.IsActive = IsActive;
			OnLastNavCommand.IsActive = IsActive;

			IsActiveChanged?.Invoke (this, new EventArgs ());
		}

		private void ValidationErrorCommandHandler ()
		{
			OnSaveCommand.RaiseCanExecuteChanged ();
		}

		private void LastNavCommandHandler ()
		{
			RoleListView.MoveCurrentToLast ();
			SelectedRole = (RoleModel)RoleListView.CurrentItem;
		}

		private void PreviousNavCommandHandler ()
		{
			RoleListView.MoveCurrentToPrevious ();

			if (RoleListView.IsCurrentBeforeFirst == true) {
				RoleListView.MoveCurrentToFirst ();
			}

			SelectedRole = (RoleModel)RoleListView.CurrentItem;
		}

		private void NextNavCommandHandler ()
		{
			RoleListView.MoveCurrentToNext ();

			if (RoleListView.IsCurrentAfterLast == true) {
				RoleListView.MoveCurrentToLast ();
			}

			SelectedRole = (RoleModel)RoleListView.CurrentItem;
		}

		private void FirstNavCommandHandler ()
		{
			RoleListView.MoveCurrentToFirst ();

			SelectedRole = (RoleModel)RoleListView.CurrentItem;
		}

		private void CancelCommandHandler ()
		{
			SetCommandEnableStatus (ExecutionTypes.Cancel);
		}

		private void AddCommandHandler ()
		{
			SetCommandEnableStatus (ExecutionTypes.Add);

			InitializeRoleInfoModel ();
		}

		private void EditCommandHandler ()
		{
			SetCommandEnableStatus (ExecutionTypes.Edit);
		}

		private void DeleteCommandHandler ()
		{
			if (SelectedRole != null) {
				DeleteRoleCommandHandler (SelectedRole);
			}
		}

		private async void DeleteRoleCommandHandler (RoleModel role)
		{
			SetCommandEnableStatus (ExecutionTypes.Delete);
			if (role != null) {
				await _roleService.RemoveRoleAsync (role.ID);
				await InitializeAsync ();
				FirstNavCommandHandler ();
			}
		}

		private async void SaveCommandHandler ()
		{
			switch (_currentExecutionType) {
				case ExecutionTypes.Add: {
						var newRoleModel = new RoleModel
						{
							Name = RoleName,
							Description = RoleDescription
						};

						var result = await _roleService.AddRoleAsync (newRoleModel);
					}
					break;

				case ExecutionTypes.Edit: {
						var result = await _roleService.GetRoleByIDAsync (RoleID);
						if (result.DidError == false) {
							RoleModel targetRole = result.Model;
							targetRole.Name = RoleName;
							targetRole.Description = RoleDescription;

							await _roleService.UpdateRoleAsync (targetRole);
						}
					}
					break;
			}

			await InitializeAsync ();

			FirstNavCommandHandler ();

			SetCommandEnableStatus (ExecutionTypes.Save);
		}
		#endregion Command Handler

		private void SetCommandEnableStatus (ExecutionTypes activeType = ExecutionTypes.None)
		{
			_currentExecutionType = activeType;

			switch (activeType) {
				case ExecutionTypes.Add:
				case ExecutionTypes.Edit:
				case ExecutionTypes.Delete: {
						CanCancelRole = true;
						CanAddRoleInfo = false;
						CanEditRole = false;
						CanDeleteRole = false;
						CanSaveRole = true;

						IsRoleListEnabled = false;

						if (activeType == ExecutionTypes.Edit) {
							IsRoleFormReadOnly = false;
						}
					}
					break;

				default: {
						CanCancelRole = false;
						CanAddRoleInfo = true;
						CanEditRole = false;
						CanDeleteRole = false;
						CanSaveRole = false;

						if (SelectedRoleIndex != -1) {
							CanEditRole = true;
							CanDeleteRole = true;
						}

						IsRoleListEnabled = true;
						IsRoleFormReadOnly = true;
					}
					break;
			}

			// Show New Form if execution type is Add
			RoleInfoFormVisibility = activeType == ExecutionTypes.Add ? true : false;
		}

		private void SetNavigationCommandEnableStatus ()
		{
			CanNavToFirst = false;
			CanNavToPrevious = false;
			CanNavToNext = false;
			CanNavToLast = false;

			if (SelectedRoleIndex < _roleListViewCount - 1) {
				CanNavToLast = true;
				CanNavToNext = true;
			}

			if (0 < SelectedRoleIndex) {
				CanNavToFirst = true;
				CanNavToPrevious = true;
			}
		}

		private void InitializeRoleInfoModel ()
		{
			RoleName = string.Empty;
			RoleDescription = string.Empty;
		}

		#endregion
	}
}