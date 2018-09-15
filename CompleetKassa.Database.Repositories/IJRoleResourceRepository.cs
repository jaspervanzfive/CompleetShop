using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompleetKassa.Database.Entities;

namespace CompleetKassa.Database.Repositories
{
    public interface IJRoleResourceRepository
	{
        IQueryable<JRoleResource> GetAll(int pageSize = 10, int pageNumber = 1);

        Task<int> AddAsync(JRoleResource entity);

        Task<int> DeleteAsync(JRoleResource entity);
    }
}
