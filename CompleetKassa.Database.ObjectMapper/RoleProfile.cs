using System.Linq;
using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.ObjectMap
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleModel, Role>();
            CreateMap<Role, RoleModel>()
                .ForMember(
                    dest => dest.Resources,
                    opt => opt.MapFrom(src => src.RoleResources.Select(rs => rs.Resource).ToList())
                 );
        }
    }
}
