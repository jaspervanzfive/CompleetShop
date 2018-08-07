using System;
using System.Reflection;
using System.Threading.Tasks;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.EF.Extensions;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompleetKassa.Database.Services
{
	public class UserService : BaseService, IUserService
	{
		public UserService (ILogger logger, IAppUser userInfo, AppDbContext dbContext)
			: base (logger, userInfo, dbContext)
		{
		}

		public async Task<IListResponse<User>> GetUsersAsync (int pageSize = 0, int pageNumber = 0)
		{
			Logger?.LogInformation (CreateInvokedMethodLog (MethodBase.GetCurrentMethod ().ReflectedType.FullName));

			var response = new ListResponse<User> ();

			try {
				response.Model = await UserRepository.GetAll (pageSize, pageNumber).ToListAsync ();
			}
			catch (Exception ex) {
				response.SetError (ex, Logger);
			}

			return response;
		}

		public async Task<ISingleResponse<User>> GetUsersByIDAsync (int	userID)
		{
			Logger?.LogInformation (CreateInvokedMethodLog (MethodBase.GetCurrentMethod ().ReflectedType.FullName));

			var response = new SingleResponse<User> ();

			try {
				response.Model = await UserRepository.GetByIDAsync (userID);
			}
			catch (Exception ex) {
				response.SetError (ex, Logger);
			}

			return response;
		}


		public async Task<ISingleResponse<User>> AddUserAsync (User details)
		{
			var response = new SingleResponse<User> ();

			using (var transaction = DbContext.Database.BeginTransaction ()) {
				try {
					await UserRepository.AddAsync (details);

					transaction.Commit ();
				}
				catch (Exception ex) {
					transaction.Rollback ();
					throw ex;
				}
			}

			return response;
		}

		public async Task<ISingleResponse<User>> UpdateUserAsync (User updates)
		{
			Logger?.LogInformation (CreateInvokedMethodLog (MethodBase.GetCurrentMethod ().ReflectedType.FullName));

			var response = new SingleResponse<User> ();

			using (var transaction = DbContext.Database.BeginTransaction ()) {
				try {
					await UserRepository.UpdateAsync (updates);

					transaction.Commit ();
					response.Model = updates;
				}
				catch (Exception ex) {
					transaction.Rollback ();
					response.SetError (ex, Logger);
				}
			}

			return response;
		}
	}
}
