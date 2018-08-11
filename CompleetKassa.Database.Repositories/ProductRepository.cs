using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
	public class ProductRepository : BaseRepository, IProductRepository
	{
		public ProductRepository(IAppUser userInfo, AppDbContext dbContext)
			: base(userInfo, dbContext)
		{
		}

		#region "Read Method"

		public async Task<Product> GetByIDAsync(int productID)
				=> await DbContext.Set<Product>().FirstOrDefaultAsync(item => item.ID == productID);

		public IQueryable<Product> GetAll(int pageSize = 10, int pageNumber = 1)
				=> DbContext.Paging<Product>(pageSize, pageNumber);
		#endregion "Read Method"

		#region "Write Method"
		public async Task<int> AddAsync(Product entity)
		{
			Add(entity);

			return await CommitChangesAsync();
		}

		public async Task<int> AddAsync(IAsyncEnumerable<Product> entities)
		{
			await entities.ForEachAsync(entity => Add(entity));

			return await CommitChangesAsync();
		}

		public async Task<int> DeleteAsync(Product entity)
		{
			Remove(entity);

			return await CommitChangesAsync();
		}

		public async Task<int> UpdateAsync(Product changes)
		{
			Update(changes);

			return await CommitChangesAsync();
		}
		#endregion "Write Method"
	}
}
