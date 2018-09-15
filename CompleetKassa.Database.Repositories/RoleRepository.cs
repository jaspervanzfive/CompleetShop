using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
	public class RoleRepository : BaseAuditRepository, IRoleRepository
	{
		public RoleRepository (IAppUser userInfo, DbContext dbContext) : base (userInfo, dbContext)
		{
		}

		#region "Read Method"

		public async Task<Role> GetByIDAsync (int roleID)
				=> await DbContext.Set<Role> ().FirstOrDefaultAsync (item => item.ID == roleID);

		public IQueryable<Role> GetAll (int pageSize = 10, int pageNumber = 1)
				=> DbContext.Paging<Role> (pageSize, pageNumber);
		#endregion "Read Method"

		#region "Write Method"
		public async Task<int> AddAsync (Role entity)
		{
			Add (entity);

			return await CommitChangesAsync ();
		}

		public async Task<int> DeleteAsync (Role entity)
		{
			Remove (entity);

			return await CommitChangesAsync ();
		}

		public async Task<int> UpdateAsync (Role changes)
		{
			Update (changes);

			return await CommitChangesAsync ();
		}
		#endregion "Write Method"
	}
}
