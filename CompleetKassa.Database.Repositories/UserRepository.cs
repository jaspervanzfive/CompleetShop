using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
	public class UserRepository : BaseAuditRepository, IUserRepository
	{
		public UserRepository (IAppUser userInfo, AppDbContext dbContext)
			: base (userInfo, dbContext)
		{
		}

        #region "Read Method"
        //public async Task<User> GetFirstOrDefaultAsync(int userID)
        //=> await DbContext.Set<User>().GetFirstOrDefault().FirstOrDefaultAsync();
        public async Task<User> GetFirstOrDefaultAsync(int userID)
        => await DbContext.Set<User>().AsNoTracking()
            .Where(u => u.ID == userID)
                .Include(u => u.UserRoles)
                .ThenInclude (ur => ur.Role)
                    .ThenInclude (urr => urr.RoleResources)
                        .ThenInclude (urrr => urrr.Resource)
            .FirstOrDefaultAsync();

        public async Task<User> GetByIDAsync (int userID)
				=> await DbContext.Set<User> ().FirstOrDefaultAsync (item => item.ID == userID);

		public async Task<User> GetByIDWithDetailsAsync (int entityID)
				=> await DbContext.Set<User> ().EagerWhere (x => x.UserCredential, m => m.ID == entityID).FirstOrDefaultAsync ();

		public IQueryable<User> GetAll (int pageSize = 10, int pageNumber = 1)
				=> DbContext.Paging<User> (pageSize, pageNumber);

		public IQueryable<User> GetAllWithDetails (int pageSize = 10, int pageNumber = 1)
				=> DbContext.Set<User> ().Paging (x => x.UserCredential);

		public IQueryable <User> GetAllDetailsWithRole (int entityID)
				=> DbContext.Set<User> ().EagerWhere (x => x.UserRoles, m => m.ID == (entityID != 0 ? entityID : 0) );

		#endregion "Read Method"

		#region "Write Method"
		public async Task<int> AddAsync (User entity)
		{
			Add (entity);

			DbContext.Set<User> ().Include (x => x.UserCredential);

			return await CommitChangesAsync ();
		}

		public async Task<int> DeleteAsync (User entity)
		{
			Remove (entity);

			return await CommitChangesAsync ();
		}

		public async Task<int> UpdateAsync (User changes)
		{
			Update (changes);

			return await CommitChangesAsync ();
		}
		#endregion "Write Method"
	}
}
