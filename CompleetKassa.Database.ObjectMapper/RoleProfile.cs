using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.ObjectMap
{
	public class RoleProfile : Profile
	{
		public RoleProfile ()
		{
			CreateMap<RoleModel, Role> ();
			CreateMap<Role, RoleModel> ();
			CreateMap<RoleModel, Resource> ();

			CreateMap<Role, RoleModel> ()
				.ForMember (
					dest => dest.Resource,
					opt => opt.MapFrom (src => src.RoleResource)
				);
		}
	}
}
