using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Database.Entities;
using CompleetKassa.Database.Repositories;
using CompleetKassa.Database.Services.Extensions;
using CompleetKassa.Log.Core;
using CompleetKassa.Models;

namespace CompleetKassa.Database.Services
{
    public class AccountService : BaseService, IAccountService
    {
        private IUserService _userService;
        private IRoleService _roleService;
        private IResourceService _resourceService;
        protected IUserRepository _userRepository { get; }
        protected IUserCredentialRepository _userCredentialRepository { get; }
        protected IJUserRoleRepository _userRoleRepository { get; }

        public AccountService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
            : base(logger, mapper, userInfo, dbContext)
        {
            _userService = new UserService(logger, mapper, userInfo, dbContext);
            _roleService = new RoleService(logger, mapper, userInfo, dbContext);
            _resourceService = new ResourceService(logger, mapper, userInfo, dbContext);

            _userRepository = new UserRepository(userInfo, dbContext);
            _userCredentialRepository = new UserCredentialRepository(userInfo, DbContext);
            _userRoleRepository = new JUserRoleRepository(userInfo, DbContext);
        }

        #region Read
        public async Task<IListResponse<UserModel>> GetUsersAsync(int pageSize = 0, int pageNumber = 0)
        {
            return await _userService.GetUsersAsync(pageSize, pageNumber);
        }

        public async Task<IListResponse<UserModel>> GetUsersWithDetailsAsync(int pageSize = 0, int pageNumber = 0)
        {
            return await _userService.GetUsersWithDetailsAsync(pageSize, pageNumber);
        }

        public async Task<ISingleResponse<UserModel>> GetFirstOrDefaultAsync (int userID)
        {
            return await _userService.GetFirstOrDefaultAsync (userID);
        }

        public async Task<ISingleResponse<UserModel>> GetUserByIDAsync(int userID)
        {
            return await _userService.GetUserByIDAsync(userID);
        }

        public async Task<ISingleResponse<UserModel>> GetUserByIDWithDetailsAsync(int userID)
        {
            return await _userService.GetUserByIDWithDetailsAsync(userID);
        }

        public async Task<IListResponse<RoleModel>> GetRolesAsync(int pageSize = 0, int pageNumber = 0)
        {
            return await _roleService.GetRolesAsync(pageSize, pageNumber);
        }

        public async Task<ISingleResponse<RoleModel>> GetRoleByIDAsync(int roleID)
        {
            return await _roleService.GetRoleByIDAsync(roleID);
        }

        public async Task<IListResponse<ResourceModel>> GetResourcesAsync(int pageSize = 0, int pageNumber = 0)
        {
            return await _resourceService.GetResourcesAsync(pageSize, pageNumber);
        }

        public async Task<ISingleResponse<ResourceModel>> GetResourceByIDAsync(int roleID)
        {
            return await _resourceService.GetResourceByIDAsync(roleID);
        }

        #endregion Read

        // Create User Account
        public async Task<ISingleResponse<UserModel>> AddUserAccountAsync(UserModel details)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));
            var response = new SingleResponse<UserModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Add User
                    var user = Mapper.Map<User>(details);
                    await _userRepository.AddAsync(user);

                    // Add User Credentials
                    var userCredential = Mapper.Map<UserCredential>(details);
                    userCredential.User = user;
                    await _userCredentialRepository.AddAsync(userCredential);

                    // Add User Roles
                    foreach (var role in details.Roles)
                    {
                        await _userRoleRepository.AddAsync(new JUserRole { UserId = user.ID, RoleId = role.ID });
                    }

                    transaction.Commit();

                    response.Model = Mapper.Map<UserModel>(user);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }

        // Create User
        public async Task<ISingleResponse<UserModel>> AddUserAsync(UserModel details)
        {
            return await _userService.AddUserAsync(details);
        }

        // Create Roles
        public async Task<ISingleResponse<RoleModel>> AddRoleAsync(RoleModel details)
        {
            return await _roleService.AddRoleAsync(details);
        }

        // Create Resources
        public async Task<ISingleResponse<ResourceModel>> AddResourceAsync(ResourceModel details)
        {
            return await _resourceService.AddResourceAsync(details);
        }

        // Create Role <-> Resources
        public async Task<ISingleResponse<RoleModel>> AddRoleResourcesAsync(RoleModel role, ICollection<ResourceModel> resources)
        {
            return await _roleService.AddRoleResourcesAsync(role, resources);
        }

        public async Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(int roleID, int resourceID)
        {
            return await _roleService.AddRoleResourceAsync(roleID, resourceID);
        }

        // Create User <-> Role
        public async Task<ISingleResponse<UserModel>> AddUserRolesAsync(UserModel user, ICollection<RoleModel> roles)
        {
            return await _userService.AddUserRolesAsync(user, roles);
        }

        public async Task<ISingleResponse<UserModel>> AddUserRoleAsync(int userID, int roleID)
        {
            return await _userService.AddUserRoleAsync(userID, roleID);
        }

        public async Task<ISingleResponse<UserModel>> UpdateUserAsync(UserModel updates)
        {
            return await _userService.UpdateUserAsync(updates);
        }

        public async Task<ISingleResponse<UserModel>> RemoveUserAsync(int userID)
        {
            return await _userService.RemoveUserAsync(userID);
        }

        public async Task<ISingleResponse<RoleModel>> UpdateRoleAsync(RoleModel updates)
        {
            return await _roleService.UpdateRoleAsync(updates);
        }

        public async Task<ISingleResponse<RoleModel>> RemoveRoleAsync(int roleID)
        {
            return await _roleService.RemoveRoleAsync(roleID);
        }

        public async Task<ISingleResponse<ResourceModel>> UpdateResourceAsync(ResourceModel updates)
        {
            return await _resourceService.UpdateResourceAsync(updates);
        }

        public async Task<ISingleResponse<ResourceModel>> RemoveResourceAsync(int roleID)
        {
            return await _resourceService.RemoveResourceAsync(roleID);
        }

    }
}
