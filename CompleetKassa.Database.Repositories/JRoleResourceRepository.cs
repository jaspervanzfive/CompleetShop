using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{

    public class JRoleResourceRepository : BaseRepository, IJRoleResourceRepository
	{
        public JRoleResourceRepository (IAppUser userInfo, DbContext dbContext) : base (userInfo, dbContext)
        {

        }

        #region "Read Method"

        public IQueryable<JRoleResource> GetAll(int pageSize = 10, int pageNumber = 1)
                => DbContext.Paging<JRoleResource>(pageSize, pageNumber);


        #endregion "Read Method"

        #region "Write Method"
        public async Task<int> AddAsync(JRoleResource entity)
        {
            DbContext.Set<JRoleResource>().Add(entity);

            return await CommitChangesAsync();
        }

        public async Task<int> DeleteAsync(JRoleResource entity)
        {
            DbContext.Set<JRoleResource>().Remove(entity);

            return await CommitChangesAsync();
        }
        #endregion "Write Method"
    }
}
