using System.Collections.Generic;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Models;

namespace CompleetKassa.Database.Services
{
    public interface IAccountService
    {


        #region Read
        Task<IListResponse<UserModel>> GetUsersAsync(int pageSize = 0, int pageNumber = 0);
        Task<IListResponse<UserModel>> GetUsersWithDetailsAsync(int pageSize = 0, int pageNumber = 0);
        Task<ISingleResponse<UserModel>> GetFirstOrDefaultAsync (int userID);
        Task<ISingleResponse<UserModel>> GetUserByIDAsync(int userID);
        Task<ISingleResponse<UserModel>> GetUserByIDWithDetailsAsync(int userID);
        Task<IListResponse<RoleModel>> GetRolesAsync(int pageSize = 0, int pageNumber = 0);
        Task<ISingleResponse<RoleModel>> GetRoleByIDAsync(int roleID);
        Task<IListResponse<ResourceModel>> GetResourcesAsync(int pageSize = 0, int pageNumber = 0);
        Task<ISingleResponse<ResourceModel>> GetResourceByIDAsync(int roleID);

        #endregion Read

        #region Write
        Task<ISingleResponse<UserModel>> AddUserAccountAsync(UserModel details);
        Task<ISingleResponse<UserModel>> AddUserAsync(UserModel details);
        Task<ISingleResponse<RoleModel>> AddRoleAsync(RoleModel details);
        Task<ISingleResponse<ResourceModel>> AddResourceAsync(ResourceModel details);
        Task<ISingleResponse<RoleModel>> AddRoleResourcesAsync(RoleModel role, ICollection<ResourceModel> resource);
        Task<ISingleResponse<RoleModel>> AddRoleResourceAsync(int roleID, int resourceID);
        Task<ISingleResponse<UserModel>> AddUserRolesAsync(UserModel user, ICollection<RoleModel> roles);
        Task<ISingleResponse<UserModel>> AddUserRoleAsync(int userID, int roleID);

        Task<ISingleResponse<UserModel>> UpdateUserAsync(UserModel updates);
        Task<ISingleResponse<UserModel>> RemoveUserAsync(int userID);

        Task<ISingleResponse<RoleModel>> UpdateRoleAsync(RoleModel updates);
        Task<ISingleResponse<RoleModel>> RemoveRoleAsync(int roleID);

        Task<ISingleResponse<ResourceModel>> UpdateResourceAsync(ResourceModel updates);
        Task<ISingleResponse<ResourceModel>> RemoveResourceAsync(int roleID);

        #endregion Write
    }
}
