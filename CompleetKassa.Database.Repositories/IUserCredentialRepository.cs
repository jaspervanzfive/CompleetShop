using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Data;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
	public interface IUserCredentialRepository : IRepository
	{
		IQueryable<UserCredential> GetAll (int pageSize = 10, int pageNumber = 1);

		Task<UserCredential> GetByIDAsync (int userID);

		Task<int> AddAsync (UserCredential entity);

		Task<int> UpdateAsync (UserCredential changes);

		Task<int> DeleteAsync (UserCredential entity);
	}
}
