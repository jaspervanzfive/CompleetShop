using System.Linq;
using System.Threading.Tasks;
using CompleetShop.Database.Core.Services;
using CompleetShop.Database.EntityLayer;

namespace CompleetShop.Database.DataLayer.Contracts
{
	public interface IUserRepository : IRepository
	{
		IQueryable<User> GetAll (int pageSize = 10, int pageNumber = 1);

		Task<User> GetByIDAsync (int userID);

		Task<int> AddAsync (User entity);

		Task<int> UpdateAsync (User changes);

		Task<int> DeleteAsync (User entity);
	}
}
