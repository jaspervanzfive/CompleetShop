using System.IO;
using System.Reflection;
using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.Database.ObjectMapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            string applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.Category,
                        opt => opt.ResolveUsing(src => (src.Category != null && src.Category.ParentCategory != null) ? src.Category.ParentCategory.Name : src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.SubCategory,
                        opt => opt.ResolveUsing(src => (src.Category != null && src.Category.ParentCategory != null) ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.ImageAbsolutePath,
                        opt => opt.MapFrom(src => Path.Combine(applicationPath, src.Image)));

            //Do not map Category and SubCategory Names
            CreateMap<ProductModel, Product>()
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());
        }
    }
}
