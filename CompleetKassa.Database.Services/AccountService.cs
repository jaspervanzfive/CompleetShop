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
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Services
{
    public class AccountService : BaseService, IAccountService
    {
        private IUserService _userService;
        private IRoleService _roleService;
        private IResourceService _resourceService;

        public AccountService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
            : base(logger, mapper, userInfo, dbContext)
        {
            _userService = new UserService(logger, mapper, userInfo, dbContext);
            _roleService = new RoleService(logger, mapper, userInfo, dbContext);
            _resourceService = new ResourceService(logger, mapper, userInfo, dbContext);
        }

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
                    await UserRepository.AddAsync(user);

                    // Add User Credentials
                    var userCredential = Mapper.Map<UserCredential>(details);
                    userCredential.User = user;
                    await UserCredentialRepository.AddAsync(userCredential);

                    // Add User Roles
                    foreach (var role in details.Roles)
                    {
                        await UserRoleRepository.AddAsync(new JUserRole { UserId = user.ID, RoleId = role.ID });
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

        // 4. Create Role <-> Resources
        public async Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(RoleModel role, ICollection<ResourceModel> resources)
        {
            return await _roleService.AddRoleResourcesAsync(role, resources);
        }

        public async Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(int roleID, int resourceID)
        {
            return await _roleService.AddRoleResourceAsync(roleID, resourceID);
        }

        // 5. Create User <-> Role

    }
}
