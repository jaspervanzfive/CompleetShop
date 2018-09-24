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
    internal class RoleService : BaseService, IRoleService
    {
        protected IRoleRepository RoleRepository { get; }
        protected IJRoleResourceRepository RoleResourceRepository { get; }

        public RoleService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
            : base(logger, mapper, userInfo, dbContext)
        {
            RoleRepository = new RoleRepository(userInfo, DbContext);
            RoleResourceRepository = new JRoleResourceRepository(userInfo, DbContext);
        }

        public async Task<IListResponse<RoleModel>> GetRolesAsync(int pageSize = 0, int pageNumber = 0)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new ListResponse<RoleModel>();

            try
            {
                response.Model = await RoleRepository.GetAll(pageSize, pageNumber).Select(o => Mapper.Map<RoleModel>(o)).ToListAsync();
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
                var userDetails = await RoleRepository.GetByIDAsync(roleID);

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
                    await RoleRepository.AddAsync(role);

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

        public async Task<ISingleResponse<RoleModel>> AddRoleResourcesAsync(RoleModel role, ICollection<ResourceModel> resources)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<RoleModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {

                    foreach (var res in resources)
                    {
                        await RoleResourceRepository.AddAsync(new JRoleResource
                        {
                            RoleID = role.ID,
                            ResourceID = res.ID
                        });
                    }

                    transaction.Commit();

                    var roleResponse = await DbContext.Set<Role>().EagerWhere(x => x.RoleResources, m => m.ID == role.ID).FirstOrDefaultAsync();

                    response.Model = Mapper.Map<RoleModel>(roleResponse);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.SetError(ex, Logger);
                }
            }

            return response;
        }

        public async Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(int roleID, int resourceID)
        {
            Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

            var response = new SingleResponse<RoleModel>();

            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    await RoleResourceRepository.AddAsync(new JRoleResource
                    {
                        RoleID = roleID,
                        ResourceID = resourceID
                    });

                    transaction.Commit();

                    var roleResponse = await DbContext.Set<Role>().EagerWhere(x => x.RoleResources, m => m.ID == roleID).FirstOrDefaultAsync();

                    response.Model = Mapper.Map<RoleModel>(roleResponse);
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

                    Role role = await RoleRepository.GetByIDAsync(updates.ID);
                    if (role == null)
                    {
                        throw new DatabaseException("User record not found.");
                    }

                    //DO NOT USE: Will set User properties to NULL if property not exists in RoleModel. Use instead: Mapper.Map(updates, user);
                    //user = Mapper.Map<User> (updates); 

                    Mapper.Map(updates, role);
                    //Mapper.Map<UserCredential> (updates);

                    await RoleRepository.UpdateAsync(role);

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
                Role role = await RoleRepository.GetByIDAsync(roleID);
                if (role == null)
                {
                    throw new DatabaseException("User record not found.");
                }

                //await UserCredentialRepository.DeleteAsync(user.UserCredential);

                await RoleRepository.DeleteAsync(role);
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
