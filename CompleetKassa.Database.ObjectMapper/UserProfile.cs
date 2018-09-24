using System.Linq;
using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.Database.ObjectMapper
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
            CreateMap<UserModel, User>();
            CreateMap<UserModel, UserCredential>();

            //CreateMap<UserModel, User> ()
            //	.ForMember<UserCredential> (
            //		dest => dest.UserCredential,
            //		opt => opt.MapFrom (src => new UserCredential
            //		{
            //			 UserName = src.UserName,
            //			 Password = src.Password
            //		})
            //	);


            CreateMap<User, UserModel>()
                .ForMember(
                    dest => dest.UserName,
                    opt => opt.MapFrom(src => src.UserCredential.UserName)
                )
                .ForMember(
                    dest => dest.Password,
                    opt => opt.MapFrom(src => src.UserCredential.Password)
                )
                .ForMember(
                    dest => dest.Roles,
                    opt => opt.MapFrom(src => src.UserRoles.Select(rs => rs.Role).ToList())
                 );
        }
	}
}
