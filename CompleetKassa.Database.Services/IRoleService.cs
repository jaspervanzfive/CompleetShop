using System.Threading.Tasks;
using CompleetKassa.Database.Core.Services;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Models;
using System.Collections.Generic;

namespace CompleetKassa.Database.Services
{
	public interface IRoleService : IService
	{
		Task<IListResponse<RoleModel>> GetRolesAsync (int pageSize = 0, int pageNumber = 0);

		Task<ISingleResponse<RoleModel>> GetRoleByIDAsync (int roleID);

		Task<ISingleResponse<RoleModel>> UpdateRoleAsync (RoleModel updates);

		Task<ISingleResponse<RoleModel>> AddRoleAsync (RoleModel details);
		Task<ISingleResponse<RoleModel>> AddRoleResourcesAsync (RoleModel role, ICollection<ResourceModel> resources);

		Task<ISingleResponse<RoleModel>> RemoveRoleAsync (int roleID);
	}
}
