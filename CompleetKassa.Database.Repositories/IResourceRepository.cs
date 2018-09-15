using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Data;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
	public interface IResourceRepository : IRepository
	{
		IQueryable<Resource> GetAll (int pageSize = 10, int pageNumber = 1);

		Task<Resource> GetByIDAsync (int roleID);

		Task<int> AddAsync (Resource entity);

		Task<int> UpdateAsync (Resource changes);

		Task<int> DeleteAsync (Resource entity);
	}
}
