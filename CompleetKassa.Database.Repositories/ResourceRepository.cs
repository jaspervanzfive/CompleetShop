using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
	public class ResourceRepository : BaseAuditRepository, IResourceRepository
	{
		public ResourceRepository (IAppUser userInfo, DbContext dbContext) : base (userInfo, dbContext)
		{
		}

		#region "Read Method"

		public async Task<Resource> GetByIDAsync (int resourceID)
				=> await DbContext.Set<Resource> ().FirstOrDefaultAsync (item => item.ID == resourceID);

		public IQueryable<Resource> GetAll (int pageSize = 10, int pageNumber = 1)
				=> DbContext.Paging<Resource> (pageSize, pageNumber);
		#endregion "Read Method"

		#region "Write Method"
		public async Task<int> AddAsync (Resource entity)
		{
			Add (entity);

			return await CommitChangesAsync ();
		}

		public async Task<int> DeleteAsync (Resource entity)
		{
			Remove (entity);

			return await CommitChangesAsync ();
		}

		public async Task<int> UpdateAsync (Resource changes)
		{
			Update (changes);

			return await CommitChangesAsync ();
		}
		#endregion "Write Method"
	}
}
