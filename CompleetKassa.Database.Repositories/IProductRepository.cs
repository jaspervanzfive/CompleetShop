using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Data;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
	public interface IProductRepository : IRepository
	{
		IQueryable<Product> GetAll(int pageSize = 10, int pageNumber = 1);
		IQueryable<Product> GetAllWithCategory(int pageSize = 10, int pageNumber = 1);
		Task<Product> GetByIDAsync(int entityID);
		Task<Product> GetByIDWithCategoryAsync(int entityID);
		Task<int> AddAsync(Product entity);
		Task<int> AddAsync(IAsyncEnumerable<Product> entities);
		Task<int> UpdateAsync(Product changes);
		Task<int> DeleteAsync(Product entity);
	}
}
