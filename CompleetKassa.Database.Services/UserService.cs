using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Exception;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Database.Entities;
using CompleetKassa.Database.Repositories;
using CompleetKassa.Database.Services.Extensions;
using CompleetKassa.Log.Core;
using CompleetKassa.Models;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Services
{
    internal class UserService : BaseService, IUserService
    {
        public IUserRepository UserRepository { get; }
        internal IUserCredentialRepository UserCredentialRepository { get; }
        internal IJUserRoleRepository UserRoleRepository { get; }

        public UserService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
            : base(logger, mapper, userInfo, dbContext)
        {
            UserRepository = new UserRepository(userInfo, DbContext);
            UserCredentialRepository = new UserCredentialRepository(userInfo, DbContext);
            UserRoleRepository = new JUserRoleRepository(userInfo, DbContext);
        }

        public async Task<IListResponse<UserModel>> GetUsersAsync(int pageSize = 0, int pageNumber = 0)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new ListResponse<UserModel>();

            try
            {
                response.Model = await UserRepository.GetAll(pageSize, pageNumber).Select(o => Mapper.Map<UserModel>(o)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<IListResponse<UserModel>> GetUsersWithDetailsAsync(int pageSize = 0, int pageNumber = 0)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new ListResponse<UserModel>();

            try
            {
                response.Model = await UserRepository.GetAllWithDetails(pageSize, pageNumber).Select(o => Mapper.Map<UserModel>(o)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<UserModel>> GetFirstOrDefaultAsync(int userID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<UserModel>();

            try
            {
                var userDetails = await UserRepository.GetFirstOrDefaultAsync(userID);

                response.Model = Mapper.Map<UserModel>(userDetails);
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<UserModel>> GetUserByIDAsync(int userID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<UserModel>();

            try
            {
                var userDetails = await UserRepository.GetByIDAsync(userID);

                response.Model = Mapper.Map<UserModel>(userDetails);
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<UserModel>> GetUserByIDWithDetailsAsync(int userID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<UserModel>();

            try
            {
                var userDetails = await UserRepository.GetByIDWithDetailsAsync(userID);

                response.Model = Mapper.Map<UserModel>(userDetails);
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<IListResponse<UserModel>> GetAllDetailsWithRoleAsync(int userID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new ListResponse<UserModel>();

            try
            {
                response.Model = await UserRepository.GetAllDetailsWithRole(userID).Select(o => Mapper.Map<UserModel>(o)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<UserModel>> AddUserAsync(UserModel details)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));
            var response = new SingleResponse<UserModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var user = Mapper.Map<User>(details);

                    await UserRepository.AddAsync(user);

                    var userCredential = Mapper.Map<UserCredential>(details);
                    userCredential.User = user;
                    await UserCredentialRepository.AddAsync(userCredential);

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

        public async Task<ISingleResponse<UserModel>> AddUserRolesAsync(UserModel user, ICollection<RoleModel> roles)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<UserModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {

                    foreach (var role in roles)
                    {
                        await UserRoleRepository.AddAsync(new JUserRole
                        {
                            UserId = user.ID,
                            RoleId = role.ID
                        });
                    }

                    transaction.Commit();

                    var userResponse = await DbContext.Set<User>().EagerWhere(x => x.UserRoles, m => m.ID == user.ID).FirstOrDefaultAsync();

                    response.Model = Mapper.Map<UserModel>(userResponse);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }

        public async Task<ISingleResponse<UserModel>> AddUserRoleAsync(int userID, int roleID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));
            var response = new SingleResponse<UserModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    await UserRoleRepository.AddAsync(new JUserRole
                    {
                        UserId = userID,
                        RoleId = roleID
                    });

                    transaction.Commit();
                    var userResponse = await DbContext.Set<User>().EagerWhere(x => x.UserRoles, m => m.ID == userID).FirstOrDefaultAsync();

                    response.Model = Mapper.Map<UserModel>(userResponse);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }

        public async Task<ISingleResponse<UserModel>> UpdateUserAsync(UserModel updates)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<UserModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {

                    User user = await UserRepository.GetByIDAsync(updates.ID);
                    if (user == null)
                    {
                        throw new DatabaseException("User record not found.");
                    }

                    //DO NOT USE: Will set User properties to NULL if property not exists in UserModel. Use instead: Mapper.Map(updates, user);
                    //user = Mapper.Map<User> (updates); 

                    Mapper.Map(updates, user);
                    //Mapper.Map<UserCredential> (updates);

                    await UserRepository.UpdateAsync(user);

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

        public async Task<ISingleResponse<UserModel>> RemoveUserAsync(int userID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<UserModel>();

            try
            {
                // Retrieve user by id
                User user = await UserRepository.GetByIDAsync(userID);
                if (user == null)
                {
                    throw new DatabaseException("User record not found.");
                }

                await UserRepository.DeleteAsync(user);
                response.Model = Mapper.Map<UserModel>(user);
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }
    }
}
