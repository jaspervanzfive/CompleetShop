using AutoMapper;
using CompleetKassa.Database.Entities;
using CompleetKassa.Models;

namespace CompleetKassa.Database.ObjectMapper
{
	public class ProductProfile : Profile
	{
		public ProductProfile()
		{
			CreateMap<Product, ProductModel>();
			CreateMap<ProductModel, Product>();
		}
	}
}
