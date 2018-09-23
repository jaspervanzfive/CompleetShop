using System.Collections.Generic;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Models;

namespace CompleetKassa.Database.Services
{
    public interface IAccountService
    {
        Task<ISingleResponse<UserModel>> AddUserAccountAsync(UserModel details);
        Task<ISingleResponse<UserModel>> AddUserAsync(UserModel details);
        Task<ISingleResponse<RoleModel>> AddRoleAsync(RoleModel details);
        Task<ISingleResponse<ResourceModel>> AddResourceAsync(ResourceModel details);
        Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(RoleModel role, ICollection<ResourceModel> resource);
        Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(int roleID, int resourceID);
    }
}
