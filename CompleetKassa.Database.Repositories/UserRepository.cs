using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IAppUser userInfo, AppDbContext dbContext)
            : base(userInfo, dbContext)
        {
        }

        #region "Read Method"

        public async Task<User> GetByIDAsync(int userID)
                => await DbContext.Set<User>().FirstOrDefaultAsync(item => item.ID == userID);

        public IQueryable<User> GetAll(int pageSize = 10, int pageNumber = 1)
                => DbContext.Paging<User>(pageSize, pageNumber);
        #endregion "Read Method"

        #region "Write Method"
        public async Task<int> AddAsync(User entity)
        {
            Add(entity);

            return await CommitChangesAsync();
        }

        public async Task<int> DeleteAsync(User entity)
        {
            Remove(entity);

            return await CommitChangesAsync();
        }

        public async Task<int> UpdateAsync(User changes)
        {
            Update(changes);

            return await CommitChangesAsync();
        }
        #endregion "Write Method"
    }
}
