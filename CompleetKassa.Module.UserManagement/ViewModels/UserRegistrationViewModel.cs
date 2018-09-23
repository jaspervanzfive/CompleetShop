using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using CompleetKassa.Database.Services;
using CompleetKassa.DataValidation;
using CompleetKassa.Models;
using CompleetKassa.Module.UserManagement.Commands;
using Microsoft.Practices.Unity;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using static CompleetKassa.Module.UserManagement.Definitions.Enumeration;

namespace CompleetKassa.Module.UserManagement.ViewModels
{

    public class UserRegistrationViewModel : ViewModelValidationBase, IActiveAware
    {
        #region Fields
        private IAccountService _accountService;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IModuleCommands _moduleCommands;

        private int _userListViewCount;
        private ExecutionTypes _currentExecutionType;
        #endregion Fields

        #region Property
        public Task Initialization { get; private set; }
        #endregion Property

        #region Command Property
        public DelegateCommand OnAddCommand { get; private set; }
        public DelegateCommand OnEditCommand { get; private set; }
        public DelegateCommand OnSaveCommand { get; private set; }
        public DelegateCommand OnDeleteCommand { get; private set; }
        public DelegateCommand OnCancelCommand { get; private set; }
        public DelegateCommand<UserModel> OnDeleteUserCommand { get; private set; }

        public DelegateCommand OnFirstNavCommand { get; private set; }
        public DelegateCommand OnPreviousNavCommand { get; private set; }
        public DelegateCommand OnNextNavCommand { get; private set; }
        public DelegateCommand OnLastNavCommand { get; private set; }

        public DelegateCommand OnValidationErrorCommand { get; private set; }
        #endregion Command Property

        #region Bindable Property
        private RoleModel _selRoleView;
        public RoleModel SelRoleView
        {
            get { return _selRoleView; }
            private set
            {
                SetProperty(ref _selRoleView, value);
            }
        }

        private ICollectionView _roleListView;
        public ICollectionView RoleListView
        {
            get { return _roleListView; }
            private set
            {
                SetProperty(ref _roleListView, value);
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool _userInfoFormVisibility;
        public bool UserInfoFormVisibility
        {
            get { return _userInfoFormVisibility; }
            set
            {
                SetProperty(ref _userInfoFormVisibility, value);
            }
        }

        private bool _isUserListEnabled;
        public bool IsUserListEnabled
        {
            get { return _isUserListEnabled; }
            set
            {
                SetProperty(ref _isUserListEnabled, value);
            }
        }

        private bool _isUserFormReadOnly;
        public bool IsUserFormReadOnly
        {
            get { return _isUserFormReadOnly; }
            set
            {
                SetProperty(ref _isUserFormReadOnly, value);
                UserFormColor = _isUserFormReadOnly == true ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.CornflowerBlue);
            }
        }

        private Brush _userFormColor;
        public Brush UserFormColor
        {
            get { return _userFormColor; }
            set
            {
                SetProperty(ref _userFormColor, value);
            }
        }

        private ICollectionView _userListView;
        public ICollectionView UserListView
        {
            get { return _userListView; }
            private set
            {
                SetProperty(ref _userListView, value);

                if (_userListView != null)
                {
                    _userListViewCount = _userListView.Cast<object>().Count();
                }

                SetNavigationCommandEnableStatus();
            }
        }

        private int _selectedUserIndex;
        public int SelectedUserIndex
        {
            get { return _selectedUserIndex; }
            set
            {
                SetProperty(ref _selectedUserIndex, value);
                SetCommandEnableStatus(ExecutionTypes.None);
                SetNavigationCommandEnableStatus();
            }
        }

        private UserModel _selectedUser;
        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                SetProperty(ref _selectedUser, value);

                UserInfoID = _selectedUser.ID;
                UserInfoFirstName = _selectedUser.FirstName;
                UserInfoMiddleName = _selectedUser.MiddleName;
                UserInfoLastName = _selectedUser.LastName;
                UserInfoUserName = _selectedUser.UserName;
                UserInfoPassword = _selectedUser.Password;

                if (_selectedUser.Roles != null)
                {
                    RoleListView = new CollectionView(_selectedUser.Roles);
                }
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }


        #region Enable/disable Command Property
        private bool _canAddUserInfo;
        public bool CanAddUserInfo
        {
            get { return _canAddUserInfo; }
            set { SetProperty(ref _canAddUserInfo, value); }
        }

        private bool _canEditUser;
        public bool CanEditUser
        {
            get { return _canEditUser; }
            set { SetProperty(ref _canEditUser, value); }
        }

        private bool _canDeleteUser;
        public bool CanDeleteUser
        {
            get { return _canDeleteUser; }
            set { SetProperty(ref _canDeleteUser, value); }
        }

        private bool _canSaveUser;
        public bool CanSaveUser
        {
            get { return _canSaveUser; }
            set { SetProperty(ref _canSaveUser, value); }
        }

        private bool _canCancelUser;
        public bool CanCancelUser
        {
            get { return _canCancelUser; }
            set { SetProperty(ref _canCancelUser, value); }
        }

        private bool _canNavToFirst;
        public bool CanNavToFirst
        {
            get { return _canNavToFirst; }
            set { SetProperty(ref _canNavToFirst, value); }
        }

        private bool _canNavToLast;
        public bool CanNavToLast
        {
            get { return _canNavToLast; }
            set { SetProperty(ref _canNavToLast, value); }
        }

        private bool _canNavToPrevious;
        public bool CanNavToPrevious
        {
            get { return _canNavToPrevious; }
            set { SetProperty(ref _canNavToPrevious, value); }
        }

        private bool _canNavToNext;
        public bool CanNavToNext
        {
            get { return _canNavToNext; }
            set { SetProperty(ref _canNavToNext, value); }
        }
        #endregion Enable/disable Command Property

        #region User Bindable Property
        private int _userInfoID;
        public int UserInfoID
        {
            get { return _userInfoID; }
            set
            {
                SetProperty(ref _userInfoID, value);

            }
        }

        private string _userInfoFirstName;
        public string UserInfoFirstName
        {
            get { return _userInfoFirstName; }
            set
            {
                SetProperty(ref _userInfoFirstName, value);
            }
        }

        private string _userInfoMiddleName;
        public string UserInfoMiddleName
        {
            get { return _userInfoMiddleName; }
            set { SetProperty(ref _userInfoMiddleName, value); }
        }

        private string _userInfoLastName;
        public string UserInfoLastName
        {
            get { return _userInfoLastName; }
            set { SetProperty(ref _userInfoLastName, value); }
        }

        private string _userInfoUserName;
        public string UserInfoUserName
        {
            get { return _userInfoUserName; }
            set { SetProperty(ref _userInfoUserName, value); }
        }

        private string _userInfoPassword;
        public string UserInfoPassword
        {
            get { return _userInfoPassword; }
            set { SetProperty(ref _userInfoPassword, value); }
        }
        #endregion User Bindable Property

        #endregion Bindable Property

        #region Event
        public event EventHandler IsActiveChanged;
        #endregion Event

        #region Constructor
        public UserRegistrationViewModel(IUnityContainer container)
        {
            _regionManager = container.Resolve<IRegionManager>();
            _accountService = container.Resolve<IAccountService>();
            _eventAggregator = container.Resolve<IEventAggregator>();
            _moduleCommands = container.Resolve<IModuleCommands>();

            InitializeFields();
            SetValidationRules();
            InitializeCommandHandler();
            SetCommandEnableStatus();

            Initialization = InitializeAsync();

            Title = "User";
        }

        #endregion Constructor

        #region Method

        #region Initialization Method
        private void InitializeFields()
        {
            _userListViewCount = 0;
            _currentExecutionType = ExecutionTypes.None;
        }

        private async Task InitializeAsync()
        {
            var userResult = await _accountService.GetUsersWithDetailsAsync ();
            if (userResult.DidError == false)
            {
                UserListView = CollectionViewSource.GetDefaultView(userResult.Model.ToList());
            }

            var roleResult = await _accountService.GetRolesAsync();
            if (roleResult.DidError == false)
            {
                RoleListView = CollectionViewSource.GetDefaultView(roleResult.Model.ToList());
            }
        }

        private void InitializeCommandHandler()
        {
            // User operations
            OnAddCommand = new DelegateCommand(AddCommandHandler).ObservesCanExecute(() => CanAddUserInfo);
            OnEditCommand = new DelegateCommand(EditCommandHandler).ObservesCanExecute(() => CanEditUser);
            OnDeleteCommand = new DelegateCommand(DeleteCommandHandler).ObservesCanExecute(() => CanDeleteUser);
            OnSaveCommand = new DelegateCommand(SaveCommandHandler).ObservesCanExecute(() => CanSaveUser);
            OnCancelCommand = new DelegateCommand(CancelCommandHandler).ObservesCanExecute(() => CanCancelUser);
            OnDeleteUserCommand = new DelegateCommand<UserModel>(DeleteUserCommandHandler).ObservesCanExecute(() => CanDeleteUser);

            // Navigation
            OnFirstNavCommand = new DelegateCommand(FirstNavCommandHandler).ObservesCanExecute(() => CanNavToFirst);
            OnPreviousNavCommand = new DelegateCommand(PreviousNavCommandHandler).ObservesCanExecute(() => CanNavToPrevious);
            OnNextNavCommand = new DelegateCommand(NextNavCommandHandler).ObservesCanExecute(() => CanNavToNext);
            OnLastNavCommand = new DelegateCommand(LastNavCommandHandler).ObservesCanExecute(() => CanNavToLast);

            OnValidationErrorCommand = new DelegateCommand(ValidationErrorCommandHandler);

            _moduleCommands.NewCommand.RegisterCommand(OnAddCommand);
            _moduleCommands.EditCommand.RegisterCommand(OnEditCommand);
            _moduleCommands.DeleteCommand.RegisterCommand(OnDeleteCommand);
            _moduleCommands.SaveCommand.RegisterCommand(OnSaveCommand);
            _moduleCommands.CancelCommand.RegisterCommand(OnCancelCommand);

            _moduleCommands.FirstNavCommand.RegisterCommand(OnFirstNavCommand);
            _moduleCommands.PreviousNavCommand.RegisterCommand(OnPreviousNavCommand);
            _moduleCommands.NextNavCommand.RegisterCommand(OnNextNavCommand);
            _moduleCommands.LastNavCommand.RegisterCommand(OnLastNavCommand);
        }

        private void InitializeUserInfoModel()
        {
            UserInfoUserName = string.Empty;
            UserInfoPassword = string.Empty;
            UserInfoFirstName = string.Empty;
            UserInfoMiddleName = string.Empty;
            UserInfoLastName = string.Empty;
        }

        private void SetValidationRules()
        {
            AddRule(() => UserInfoFirstName, () => (string.IsNullOrEmpty(UserInfoFirstName)), "First Name is required.");
            AddRule(() => UserInfoLastName, () => (string.IsNullOrEmpty(UserInfoLastName)), "Last Name is required.");
            AddRule(() => UserInfoUserName, () => (string.IsNullOrEmpty(UserInfoUserName)), "Username is required.");
            AddRule(() => UserInfoPassword, () => (string.IsNullOrEmpty(UserInfoPassword)), "Password is required.");
        }

        #endregion Initialization Method

        #region Command Handler
        private void OnIsActiveChanged()
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

            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        private void ValidationErrorCommandHandler()
        {
            OnSaveCommand.RaiseCanExecuteChanged();
        }

        private void LastNavCommandHandler()
        {
            UserListView.MoveCurrentToLast();
            SelectedUser = (UserModel)UserListView.CurrentItem;
        }

        private void PreviousNavCommandHandler()
        {
            UserListView.MoveCurrentToPrevious();

            if (UserListView.IsCurrentBeforeFirst == true)
            {
                UserListView.MoveCurrentToFirst();
            }

            SelectedUser = (UserModel)UserListView.CurrentItem;
        }

        private void NextNavCommandHandler()
        {
            UserListView.MoveCurrentToNext();

            if (UserListView.IsCurrentAfterLast == true)
            {
                UserListView.MoveCurrentToLast();
            }

            SelectedUser = (UserModel)UserListView.CurrentItem;
        }

        private void FirstNavCommandHandler()
        {
            UserListView.MoveCurrentToFirst();

            SelectedUser = (UserModel)UserListView.CurrentItem;
        }

        private void CancelCommandHandler()
        {
            SetCommandEnableStatus(ExecutionTypes.Cancel);
        }

        private void AddCommandHandler()
        {
            SetCommandEnableStatus(ExecutionTypes.Add);

            InitializeUserInfoModel();
        }

        private void EditCommandHandler()
        {
            SetCommandEnableStatus(ExecutionTypes.Edit);
        }

        private void DeleteCommandHandler()
        {
            if (SelectedUser != null)
            {
                DeleteUserCommandHandler(SelectedUser);
            }
        }

        private async void DeleteUserCommandHandler(UserModel user)
        {
            SetCommandEnableStatus(ExecutionTypes.Delete);
            if (user != null)
            {
                await _accountService.RemoveUserAsync(user.ID);
                await InitializeAsync();
                FirstNavCommandHandler();
            }
        }

        private async void SaveCommandHandler()
        {
            switch (_currentExecutionType)
            {
                case ExecutionTypes.Add:
                    {
                        var newUserModel = new UserModel
                        {
                            UserName = UserInfoUserName,
                            Password = UserInfoPassword,
                            FirstName = UserInfoFirstName,
                            MiddleName = UserInfoMiddleName,
                            LastName = UserInfoLastName
                        };

                        var result = await _accountService.AddUserAsync(newUserModel);
                    }
                    break;

                case ExecutionTypes.Edit:
                    {
                        var result = await _accountService.GetUserByIDAsync(UserInfoID);
                        if (result.DidError == false)
                        {
                            UserModel targetUser = result.Model;
                            targetUser.UserName = UserInfoUserName;
                            targetUser.Password = UserInfoPassword;
                            targetUser.FirstName = UserInfoFirstName;
                            targetUser.MiddleName = UserInfoMiddleName;
                            targetUser.LastName = UserInfoLastName;

                            await _accountService.UpdateUserAsync(targetUser);
                        }
                    }
                    break;
            }

            await InitializeAsync();

            FirstNavCommandHandler();

            SetCommandEnableStatus(ExecutionTypes.Save);
        }
        #endregion Command Handler

        #region Helper Method
        private void SetCommandEnableStatus(ExecutionTypes activeType = ExecutionTypes.None)
        {
            _currentExecutionType = activeType;

            switch (activeType)
            {
                case ExecutionTypes.Add:
                case ExecutionTypes.Edit:
                case ExecutionTypes.Delete:
                    {
                        CanCancelUser = true;
                        CanAddUserInfo = false;
                        CanEditUser = false;
                        CanDeleteUser = false;
                        CanSaveUser = true;

                        IsUserListEnabled = false;

                        if (activeType == ExecutionTypes.Edit)
                        {
                            IsUserFormReadOnly = false;
                        }
                    }
                    break;

                default:
                    {
                        CanCancelUser = false;
                        CanAddUserInfo = true;
                        CanEditUser = false;
                        CanDeleteUser = false;
                        CanSaveUser = false;

                        if (SelectedUserIndex != -1)
                        {
                            CanEditUser = true;
                            CanDeleteUser = true;
                        }

                        IsUserListEnabled = true;
                        IsUserFormReadOnly = true;
                    }
                    break;
            }

            // Show New Form if execution type is Add
            UserInfoFormVisibility = activeType == ExecutionTypes.Add ? true : false;
        }

        private void SetNavigationCommandEnableStatus()
        {
            CanNavToFirst = false;
            CanNavToPrevious = false;
            CanNavToNext = false;
            CanNavToLast = false;

            if (SelectedUserIndex < _userListViewCount - 1)
            {
                CanNavToLast = true;
                CanNavToNext = true;
            }

            if (0 < SelectedUserIndex)
            {
                CanNavToFirst = true;
                CanNavToPrevious = true;
            }
        }

        #endregion Helper Method
        #endregion Method
    }
}