using System.Threading.Tasks;
using CompleetKassa.Database.Core.Services;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Models;

namespace CompleetKassa.Database.Services
{
	public interface IUserService : IService
	{
		Task<IListResponse<UserModel>> GetUsersAsync (int pageSize = 0, int pageNumber = 0);

		Task<ISingleResponse<UserModel>> GetUsersByIDAsync (int userID);

		Task<ISingleResponse<UserModel>> UpdateUserAsync (UserModel updates);

		Task<ISingleResponse<UserModel>> AddUserAsync (UserModel details);

		Task<ISingleResponse<UserModel>> RemoveUserAsync(int userID);
	}
}
