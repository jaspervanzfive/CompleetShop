using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
    public interface IJUserRoleRepository
    {
        IQueryable<JUserRole> GetAll(int pageSize = 10, int pageNumber = 1);

        Task<int> AddAsync(JUserRole entity);

        Task<int> DeleteAsync(JUserRole entity);
    }
}
