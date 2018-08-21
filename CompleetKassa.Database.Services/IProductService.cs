using System.Collections.Generic;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Services;
using CompleetKassa.Database.Core.Services.ResponseTypes;
using CompleetKassa.Models;

namespace CompleetKassa.Database.Services
{
	public interface IProductService : IService
	{
		Task<IListResponse<ProductModel>> GetProductsAsync(int pageSize = 0, int pageNumber = 0);
		Task<IListResponse<ProductModel>> GetProductsWithCategoryAsync(int pageSize = 0, int pageNumber = 0);

		Task<ISingleResponse<ProductModel>> GetProductByIDAsync(int productID);
		Task<ISingleResponse<ProductModel>> GetProductByIDWithCategoryAsync(int productID);

		Task<ISingleResponse<ProductModel>> UpdateProductAsync(ProductModel updates);

		Task<ISingleResponse<ProductModel>> AddProductAsync(ProductModel details);

		Task<IListResponse<ProductModel>> AddProductsAsync(IEnumerable<ProductModel> details);

		Task<ISingleResponse<ProductModel>> RemoveProductAsync(int productID);
	}
}