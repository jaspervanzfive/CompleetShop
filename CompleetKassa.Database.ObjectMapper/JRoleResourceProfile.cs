using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.Database.ObjectMapper
{
    public class JRoleResourceProfile : Profile
    {
        public JRoleResourceProfile()
        {
            CreateMap<JRoleResource, RoleModel>();

            CreateMap<JRoleResource, ResourceModel>();
        }
    }
}
