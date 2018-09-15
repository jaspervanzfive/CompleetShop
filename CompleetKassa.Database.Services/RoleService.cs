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
    public class RoleService : BaseService, IRoleService
    {
        protected IRoleRepository _roleRepository;

        public RoleService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
            : base(logger, mapper, userInfo, dbContext)
        {
            _roleRepository = new RoleRepository(userInfo, this.DbContext);
        }

        public async Task<IListResponse<RoleModel>> GetRolesAsync(int pageSize = 0, int pageNumber = 0)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new ListResponse<RoleModel>();

            try
            {
                response.Model = await _roleRepository.GetAll(pageSize, pageNumber).Select(o => Mapper.Map<RoleModel>(o)).ToListAsync();
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<RoleModel>> GetRoleByIDAsync(int roleID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<RoleModel>();

            try
            {
                var userDetails = await _roleRepository.GetByIDAsync(roleID);

                response.Model = Mapper.Map<RoleModel>(userDetails);
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }

        public async Task<ISingleResponse<RoleModel>> AddRoleAsync(RoleModel details)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));
            var response = new SingleResponse<RoleModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {

                    var role = Mapper.Map<Role>(details);
                    await _roleRepository.AddAsync(role);

                    transaction.Commit();
                    response.Model = Mapper.Map<RoleModel>(role);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return response;
        }

        public async Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(RoleModel role, ICollection<ResourceModel> resource)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var userRoleRepository = new JRoleResourceRepository(UserInfo, this.DbContext);
            var response = new SingleResponse<RoleModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var userInfo = Mapper.Map<Role>(role);
                    var userRoles = new List<JRoleResource>();
                    foreach (var res in resource)
                    {
                        await userRoleRepository.AddAsync(new JRoleResource
                        {
                            RoleID = role.ID,
                            ResourceID = res.ID
                        });
                    }

                    transaction.Commit();
                    response.Model = Mapper.Map<RoleModel>(role);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }

        public async Task<ISingleResponse<RoleModel>> UpdateRoleAsync(RoleModel updates)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<RoleModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {

                    Role role = await _roleRepository.GetByIDAsync(updates.ID);
                    if (role == null)
                    {
                        throw new DatabaseException("User record not found.");
                    }

                    //DO NOT USE: Will set User properties to NULL if property not exists in RoleModel. Use instead: Mapper.Map(updates, user);
                    //user = Mapper.Map<User> (updates); 

                    Mapper.Map(updates, role);
                    //Mapper.Map<UserCredential> (updates);

                    await _roleRepository.UpdateAsync(role);

                    transaction.Commit();
                    response.Model = Mapper.Map<RoleModel>(role);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }

        public async Task<ISingleResponse<RoleModel>> RemoveRoleAsync(int roleID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<RoleModel>();

            try
            {
                // Retrieve user by id
                Role role = await _roleRepository.GetByIDAsync(roleID);
                if (role == null)
                {
                    throw new DatabaseException("User record not found.");
                }

                //await UserCredentialRepository.DeleteAsync(user.UserCredential);

                await _roleRepository.DeleteAsync(role);
                response.Model = Mapper.Map<RoleModel>(role);
            }
            catch (Exception ex)
            {
                response.SetError(ex, Logger);
            }

            return response;
        }
    }
}
