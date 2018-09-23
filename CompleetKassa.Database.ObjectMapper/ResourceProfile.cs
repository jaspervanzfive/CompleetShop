using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.ObjectMap
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {
            CreateMap<ResourceModel, Resource>();
            CreateMap<Resource, ResourceModel>();
        }
    }
}
