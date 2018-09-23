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
	public class ResourceRegistrationViewModel : ViewModelValidationBase, IActiveAware
	{
		#region Fields
		private IAccountService _accountService;
		private IEventAggregator _eventAggregator;
		private IRegionManager _regionManager;
		IModuleCommands _moduleCommands;

		private int _resourceListViewCount;
		private ExecutionTypes _currentExecutionType;
		#endregion Fields

		#region Property
		public Task Initialization { get; private set; }
		#endregion Property

		#region Enable/disable Command Property
		private bool _canAddResourceInfo;
		public bool CanAddResourceInfo
		{
			get { return _canAddResourceInfo; }
			set { SetProperty (ref _canAddResourceInfo, value); }
		}

		private bool _canEditResource;
		public bool CanEditResource
		{
			get { return _canEditResource; }
			set { SetProperty (ref _canEditResource, value); }
		}

		private bool _canDeleteResource;
		public bool CanDeleteResource
		{
			get { return _canDeleteResource; }
			set { SetProperty (ref _canDeleteResource, value); }
		}

		private bool _canSaveResource;
		public bool CanSaveResource
		{
			get { return _canSaveResource; }
			set { SetProperty (ref _canSaveResource, value); }
		}

		private bool _canCancelResource;
		public bool CanCancelResource
		{
			get { return _canCancelResource; }
			set { SetProperty (ref _canCancelResource, value); }
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
		public DelegateCommand<ResourceModel> OnDeleteResourceCommand { get; private set; }

		public DelegateCommand OnFirstNavCommand { get; private set; }
		public DelegateCommand OnPreviousNavCommand { get; private set; }
		public DelegateCommand OnNextNavCommand { get; private set; }
		public DelegateCommand OnLastNavCommand { get; private set; }

		public DelegateCommand OnValidationErrorCommand { get; private set; }
		#endregion Command Property

		#region Bindable Property
		private int _resourceID;
		public int ResourceID
		{
			get { return _resourceID; }
			set
			{
				SetProperty (ref _resourceID, value);

			}
		}

		private string _resourceName;
		public string ResourceName
		{
			get { return _resourceName; }
			set
			{
				SetProperty (ref _resourceName, value);
			}
		}

		private string _resourceDescription;
		public string ResourceDescription
		{
			get { return _resourceDescription; }
			set { SetProperty (ref _resourceDescription, value); }
		}

		private ICollectionView _resourceListView;
		public ICollectionView ResourceListView
		{
			get { return _resourceListView; }
			private set
			{

				SetProperty (ref _resourceListView, value);

				if (_resourceListView != null) {
					_resourceListViewCount = _resourceListView.Cast<object> ().Count ();
				}
			}
		}

		private int _selectedResourceIndex;
		public int SelectedResourceIndex
		{
			get { return _selectedResourceIndex; }
			set
			{
				SetProperty (ref _selectedResourceIndex, value);
				SetCommandEnableStatus (ExecutionTypes.None);
				SetNavigationCommandEnableStatus ();
			}
		}

		private ResourceModel _selectedResource;
		public ResourceModel SelectedResource
		{
			get { return _selectedResource; }
			set
			{
				SetProperty (ref _selectedResource, value);

				ResourceID = _selectedResource.ID;
				ResourceName = _selectedResource.Name;
				ResourceDescription = _selectedResource.Description;

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

		private bool _resourceInfoFormVisibility;
		public bool ResourceInfoFormVisibility
		{
			get { return _resourceInfoFormVisibility; }
			set
			{
				SetProperty (ref _resourceInfoFormVisibility, value);
			}
		}

		private bool _isResourceListEnabled;
		public bool IsResourceListEnabled
		{
			get { return _isResourceListEnabled; }
			set
			{
				SetProperty (ref _isResourceListEnabled, value);
			}
		}

		private bool _isResourceFormReadOnly;
		public bool IsResourceFormReadOnly
		{
			get { return _isResourceFormReadOnly; }
			set
			{
				SetProperty (ref _isResourceFormReadOnly, value);
				ResourceFormColor = _isResourceFormReadOnly == true ? new SolidColorBrush (Colors.LightBlue) : new SolidColorBrush (Colors.CornflowerBlue);
			}
		}

		private Brush _resourceFormColor;
		public Brush ResourceFormColor
		{
			get { return _resourceFormColor; }
			set
			{
				SetProperty (ref _resourceFormColor, value);
			}
		}
		#endregion Bindable Property

		public event EventHandler IsActiveChanged;

		#region Constructor
		public ResourceRegistrationViewModel (IUnityContainer container)
		{
			_regionManager = container.Resolve<IRegionManager> ();
			_accountService = container.Resolve<IAccountService> ();
			_eventAggregator = container.Resolve<IEventAggregator> ();
			_moduleCommands = container.Resolve<IModuleCommands> ();

			InitializeFields ();
			InitializeCommandHandler ();
			SetCommandEnableStatus ();

			Initialization = InitializeAsync ();

			Title = "Resource";
		}
		#endregion Constructor

		#region Methods
		private void InitializeFields ()
		{
			_resourceListViewCount = 0;
			_currentExecutionType = ExecutionTypes.None;
		}
		private void InitializeCommandHandler ()
		{
			// Resource operations
			OnAddCommand = new DelegateCommand (AddCommandHandler).ObservesCanExecute (() => CanAddResourceInfo);
			OnEditCommand = new DelegateCommand (EditCommandHandler).ObservesCanExecute (() => CanEditResource);
			OnDeleteCommand = new DelegateCommand (DeleteCommandHandler).ObservesCanExecute (() => CanDeleteResource);
			OnSaveCommand = new DelegateCommand (SaveCommandHandler).ObservesCanExecute (() => CanSaveResource);
			OnCancelCommand = new DelegateCommand (CancelCommandHandler).ObservesCanExecute (() => CanCancelResource);
			OnDeleteResourceCommand = new DelegateCommand<ResourceModel> (DeleteResourceCommandHandler).ObservesCanExecute (() => CanDeleteResource);

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
			var result = await _accountService.GetResourcesAsync ();
			if (result.DidError == false) {
				ResourceListView = CollectionViewSource.GetDefaultView (result.Model.ToList ());
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
			ResourceListView.MoveCurrentToLast ();
			SelectedResource = (ResourceModel)ResourceListView.CurrentItem;
		}

		private void PreviousNavCommandHandler ()
		{
			ResourceListView.MoveCurrentToPrevious ();

			if (ResourceListView.IsCurrentBeforeFirst == true) {
				ResourceListView.MoveCurrentToFirst ();
			}

			SelectedResource = (ResourceModel)ResourceListView.CurrentItem;
		}

		private void NextNavCommandHandler ()
		{
			ResourceListView.MoveCurrentToNext ();

			if (ResourceListView.IsCurrentAfterLast == true) {
				ResourceListView.MoveCurrentToLast ();
			}

			SelectedResource = (ResourceModel)ResourceListView.CurrentItem;
		}

		private void FirstNavCommandHandler ()
		{
			ResourceListView.MoveCurrentToFirst ();

			SelectedResource = (ResourceModel)ResourceListView.CurrentItem;
		}

		private void CancelCommandHandler ()
		{
			SetCommandEnableStatus (ExecutionTypes.Cancel);
		}

		private void AddCommandHandler ()
		{
			SetCommandEnableStatus (ExecutionTypes.Add);

			InitializeResourceInfoModel ();
		}

		private void EditCommandHandler ()
		{
			SetCommandEnableStatus (ExecutionTypes.Edit);
		}

		private void DeleteCommandHandler ()
		{
			if (SelectedResource != null) {
				DeleteResourceCommandHandler (SelectedResource);
			}
		}

		private async void DeleteResourceCommandHandler (ResourceModel resource)
		{
			SetCommandEnableStatus (ExecutionTypes.Delete);
			if (resource != null) {
				await _accountService.RemoveResourceAsync (resource.ID);
				await InitializeAsync ();
				FirstNavCommandHandler ();
			}
		}

		private async void SaveCommandHandler ()
		{
			switch (_currentExecutionType) {
				case ExecutionTypes.Add: {
						var newResourceModel = new ResourceModel
						{
							Name = ResourceName,
							Description = ResourceDescription
						};

						var result = await _accountService.AddResourceAsync (newResourceModel);
					}
					break;

				case ExecutionTypes.Edit: {
						var result = await _accountService.GetResourceByIDAsync (ResourceID);
						if (result.DidError == false) {
							ResourceModel targetResource = result.Model;
							targetResource.Name = ResourceName;
							targetResource.Description = ResourceDescription;

							await _accountService.UpdateResourceAsync (targetResource);
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
						CanCancelResource = true;
						CanAddResourceInfo = false;
						CanEditResource = false;
						CanDeleteResource = false;
						CanSaveResource = true;

						IsResourceListEnabled = false;

						if (activeType == ExecutionTypes.Edit) {
							IsResourceFormReadOnly = false;
						}
					}
					break;

				default: {
						CanCancelResource = false;
						CanAddResourceInfo = true;
						CanEditResource = false;
						CanDeleteResource = false;
						CanSaveResource = false;

						if (SelectedResourceIndex != -1) {
							CanEditResource = true;
							CanDeleteResource = true;
						}

						IsResourceListEnabled = true;
						IsResourceFormReadOnly = true;
					}
					break;
			}

			// Show New Form if execution type is Add
			ResourceInfoFormVisibility = activeType == ExecutionTypes.Add ? true : false;
		}

		private void SetNavigationCommandEnableStatus ()
		{
			CanNavToFirst = false;
			CanNavToPrevious = false;
			CanNavToNext = false;
			CanNavToLast = false;

			if (SelectedResourceIndex < _resourceListViewCount - 1) {
				CanNavToLast = true;
				CanNavToNext = true;
			}

			if (0 < SelectedResourceIndex) {
				CanNavToFirst = true;
				CanNavToPrevious = true;
			}
		}

		private void InitializeResourceInfoModel ()
		{
			ResourceName = string.Empty;
			ResourceDescription = string.Empty;
		}

		#endregion
	}
}