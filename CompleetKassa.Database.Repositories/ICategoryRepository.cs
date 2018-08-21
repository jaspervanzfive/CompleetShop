using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Data;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
	public interface ICategoryRepository : IRepository
	{
		IQueryable<Category> GetAll(int pageSize = 10, int pageNumber = 1);
		IQueryable<Category> GetAllWithParentCategory(int pageSize = 10, int pageNumber = 1);
		Task<Category> GetByIDAsync(int entityID);
		Task<Category> GetByIDWithParentCategoryAsync(int entityID);
		Task<int> AddAsync(Category entity);
		Task<int> AddAsync(IAsyncEnumerable<Category> entities);
		Task<int> UpdateAsync(Category changes);
		Task<int> DeleteAsync(Category entity);
	}
}
