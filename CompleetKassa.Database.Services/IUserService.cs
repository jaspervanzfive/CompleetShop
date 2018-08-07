using System.Threading.Tasks;
using CompleetKassa.Database.Core.Services;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Services
{
	public interface IUserService : IService
	{
		Task<IListResponse<User>> GetUsersAsync (int pageSize = 0, int pageNumber = 0);

		Task<ISingleResponse<User>> GetUsersByIDAsync (int userID);

		Task<ISingleResponse<User>> UpdateUserAsync (User updates);

		Task<ISingleResponse<User>> AddUserAsync (User details);
	}
}
