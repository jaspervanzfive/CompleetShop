using System;
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
	public class ResourceService : BaseService, IResourceService
	{
		protected IResourceRepository _resourceRepository;

		public ResourceService (ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
			: base (logger, mapper, userInfo, dbContext)
		{
			_resourceRepository = new ResourceRepository(userInfo, this.DbContext);
		}

		public async Task<IListResponse<ResourceModel>> GetResourcesAsync (int pageSize = 0, int pageNumber = 0)
		{
			Logger.Info (CreateInvokedMethodLog (MethodBase.GetCurrentMethod ().ReflectedType.FullName));

			var response = new ListResponse<ResourceModel> ();

			try {
				response.Model = await _resourceRepository.GetAll (pageSize, pageNumber).Select (o => Mapper.Map<ResourceModel> (o)).ToListAsync ();
			}
			catch (Exception ex) {
				response.SetError (ex, Logger);
			}

			return response;
		}

		public async Task<ISingleResponse<ResourceModel>> GetResourceByIDAsync (int resourceID)
		{
			Logger.Info (CreateInvokedMethodLog (MethodBase.GetCurrentMethod ().ReflectedType.FullName));

			var response = new SingleResponse<ResourceModel> ();

			try {
				var userDetails = await _resourceRepository.GetByIDAsync(resourceID);

				response.Model = Mapper.Map<ResourceModel> (userDetails);
			}
			catch (Exception ex) {
				response.SetError (ex, Logger);
			}

			return response;
		}

		public async Task<ISingleResponse<ResourceModel>> AddResourceAsync (ResourceModel details)
		{
			Logger.Info (CreateInvokedMethodLog (MethodBase.GetCurrentMethod ().ReflectedType.FullName));
			var response = new SingleResponse<ResourceModel> ();

			using (var transaction = DbContext.Database.BeginTransaction ()) {
				try {

					var resource = Mapper.Map <Resource>(details);
					await _resourceRepository.AddAsync(resource);

					transaction.Commit ();
					response.Model = Mapper.Map<ResourceModel> (resource);
				}
				catch (Exception ex) {
					transaction.Rollback ();
					throw ex;
				}
			}

			return response;
		}

		public async Task<ISingleResponse<ResourceModel>> UpdateResourceAsync (ResourceModel updates)
		{
			Logger.Info (CreateInvokedMethodLog (MethodBase.GetCurrentMethod ().ReflectedType.FullName));

			var response = new SingleResponse<ResourceModel> ();

			using (var transaction = DbContext.Database.BeginTransaction ()) {
				try {

					Resource resource = await _resourceRepository.GetByIDAsync (updates.ID);
					if(resource == null) {
						throw new DatabaseException ("User record not found.");
					}

					//DO NOT USE: Will set User properties to NULL if property not exists in ResourceModel. Use instead: Mapper.Map(updates, user);
					//user = Mapper.Map<User> (updates); 

					Mapper.Map(updates, resource);
					//Mapper.Map<UserCredential> (updates);

					await _resourceRepository.UpdateAsync (resource);

					transaction.Commit ();
					response.Model = Mapper.Map<ResourceModel>(resource);
				}
				catch (Exception ex) {
					transaction.Rollback ();
					response.SetError (ex, Logger);
				}
			}

			return response;
		}

		public async Task<ISingleResponse<ResourceModel>> RemoveResourceAsync(int resourceID)
		{
			Logger.Info(CreateInvokedMethodLog(MethodBase.GetCurrentMethod().ReflectedType.FullName));

			var response = new SingleResponse<ResourceModel>();

			try
			{
				// Retrieve user by id
				Resource resource = await _resourceRepository.GetByIDAsync(resourceID);
				if (resource == null)
				{
					throw new DatabaseException("User record not found.");
				}

				//await UserCredentialRepository.DeleteAsync(user.UserCredential);

				await _resourceRepository.DeleteAsync(resource);
				response.Model = Mapper.Map<ResourceModel>(resource);
			}
			catch (Exception ex)
			{
				response.SetError(ex, Logger);
			}

			return response;
		}
	}
}
