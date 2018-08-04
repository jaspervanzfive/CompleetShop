using CompleetShop.Database.BusinessLayer.Contracts;
using CompleetShop.Database.Core.Contracts;
using CompleetShop.Database.Core.EF.Extensions;
using CompleetShop.Database.Core.Responses;
using CompleetShop.Database.DataLayer;
using CompleetShop.Database.EntityLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CompleetShop.Database.BusinessLayer.Services
{
    public class UserService : ServiceBase, IUserService
	{
		public UserService (ILogger logger, IUserInfo userInfo, AppDbContext dbContext)
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
