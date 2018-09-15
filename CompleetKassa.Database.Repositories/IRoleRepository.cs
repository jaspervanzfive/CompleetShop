using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Data;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
	public interface IRoleRepository : IRepository
	{
		IQueryable<Role> GetAll (int pageSize = 10, int pageNumber = 1);

		Task<Role> GetByIDAsync (int roleID);

		Task<int> AddAsync (Role entity);

		Task<int> UpdateAsync (Role changes);

		Task<int> DeleteAsync (Role entity);
	}
}
