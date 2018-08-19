using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
	public class CategoryRepository : BaseRepository, ICategoryRepository
	{
		public CategoryRepository(IAppUser CategoryInfo, AppDbContext dbContext)
				   : base(CategoryInfo, dbContext)
		{
		}

		#region "Read Method"

		public async Task<Category> GetByIDAsync(int entityID)
				=> await DbContext.Set<Category>().FirstOrDefaultAsync(item => item.ID == entityID);

		public IQueryable<Category> GetAll(int pageSize = 10, int pageNumber = 1)
				=> DbContext.Paging<Category>(pageSize, pageNumber);
		#endregion "Read Method"

		#region "Write Method"
		public async Task<int> AddAsync(Category entity)
		{
			Add(entity);

			return await CommitChangesAsync();
		}

		public async Task<int> AddAsync(IAsyncEnumerable<Category> entities)
		{
			await entities.ForEachAsync(entity => Add(entity));

			return await CommitChangesAsync();
		}

		public async Task<int> DeleteAsync(Category entity)
		{
			Remove(entity);

			return await CommitChangesAsync();
		}

		public async Task<int> UpdateAsync(Category changes)
		{
			Update(changes);

			return await CommitChangesAsync();
		}
		#endregion "Write Method"
	}
}
