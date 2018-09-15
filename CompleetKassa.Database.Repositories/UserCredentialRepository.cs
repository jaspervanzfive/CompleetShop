using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
	public class UserCredentialRepository : BaseAuditRepository, IUserCredentialRepository
	{
		public UserCredentialRepository (IAppUser userInfo, DbContext dbContext) : base (userInfo, dbContext)
		{
		}

		#region "Read Method"

		public async Task<UserCredential> GetByIDAsync (int userID)
				=> await DbContext.Set<UserCredential> ().FirstOrDefaultAsync (item => item.ID == userID);

		public IQueryable<UserCredential> GetAll (int pageSize = 10, int pageNumber = 1)
				=> DbContext.Paging<UserCredential> (pageSize, pageNumber);
		#endregion "Read Method"

		#region "Write Method"
		public async Task<int> AddAsync (UserCredential entity)
		{
			Add (entity);

			return await CommitChangesAsync ();
		}

		public async Task<int> DeleteAsync (UserCredential entity)
		{
			Remove (entity);

			return await CommitChangesAsync ();
		}

		public async Task<int> UpdateAsync (UserCredential changes)
		{
			Update (changes);

			return await CommitChangesAsync ();
		}
		#endregion "Write Method"
	}
}
