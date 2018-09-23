using System.Threading.Tasks;
using CompleetKassa.Database.Core.Services;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Models;

namespace CompleetKassa.Database.Services
{
    internal interface IResourceService : IService
    {
        Task<IListResponse<ResourceModel>> GetResourcesAsync(int pageSize = 0, int pageNumber = 0);

        Task<ISingleResponse<ResourceModel>> GetResourceByIDAsync(int roleID);

        Task<ISingleResponse<ResourceModel>> UpdateResourceAsync(ResourceModel updates);

        Task<ISingleResponse<ResourceModel>> AddResourceAsync(ResourceModel details);

        Task<ISingleResponse<ResourceModel>> RemoveResourceAsync(int roleID);
    }
}
