using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.Database.ObjectMapper
{
	public class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<Category, CategoryModel>()
				.ForMember(
					dest => dest.Parent,
					opt => opt.MapFrom (src => src.ParentCategoryID)
				);


			CreateMap<CategoryModel, Category>()
				.ForMember(
					dest => dest.ParentCategoryID,
					opt => opt.MapFrom(src => src.Parent == 0 ? (int?)null : src.Parent)
				);
		}
	}
}
