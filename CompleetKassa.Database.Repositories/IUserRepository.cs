using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Data;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
    public interface IUserRepository : IRepository
    {
        IQueryable<User> GetAll(int pageSize = 10, int pageNumber = 1);

        Task<User> GetByIDAsync(int entityID);

        Task<int> AddAsync(User entity);

        Task<int> UpdateAsync(User changes);

        Task<int> DeleteAsync(User entity);
    }
}
