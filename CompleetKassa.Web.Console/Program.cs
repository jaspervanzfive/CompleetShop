using System.Threading.Tasks;
using CompleetKassa.Web.Services;

namespace CompleetKassa.Web.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			RunAsync().GetAwaiter().GetResult();
		}

		static async Task RunAsync()
		{
			var productWebService = new ProductService(@"http://api.shoppreview.nl/public/api/");
			var products = await productWebService.GetProductsAsync("products");

			foreach (var product in products)
			{
				System.Console.WriteLine(product.Price);
			}

			System.Console.ReadKey();
		}
	}
}
