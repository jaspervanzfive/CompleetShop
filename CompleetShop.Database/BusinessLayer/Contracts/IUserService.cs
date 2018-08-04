using System.Threading.Tasks;
using CompleetShop.Database.Core.Contracts;
using CompleetShop.Database.Core.Services;
using CompleetShop.Database.EntityLayer;

namespace CompleetShop.Database.BusinessLayer.Contracts
{
	public interface IUserService : IService
	{
		Task<IListResponse<User>> GetUsersAsync (int pageSize = 0, int pageNumber = 0);

		Task<ISingleResponse<User>> GetUsersByIDAsync (int userID);

		Task<ISingleResponse<User>> UpdateUserAsync (User updates);

		Task<ISingleResponse<User>> AddUserAsync (User details);
	}
}
