using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CompleetKassa.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CompleetKassa.Web.Services
{
	public class ProductService
	{

		HttpClient _webClient;
		public ProductService(string url)
		{
			_webClient = new HttpClient();

			_webClient.BaseAddress = new Uri(url);
			_webClient.DefaultRequestHeaders.Accept.Clear();
			_webClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			_webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "a3e9ad5d90d3098d3c8716687bd45180ed67b7f2675f90fbf478c0f51796");
		}

		public async Task<IEnumerable<ProductModel>> GetProductsAsync(string path)
		{
			List<ProductModel> products = new List<ProductModel>();
			HttpResponseMessage response = await _webClient.GetAsync(path);
			if (response.IsSuccessStatusCode)
			{
				var productsJson = await response.Content.ReadAsStringAsync();
				Trace.WriteLine(productsJson);

				JObject productsSearch = JObject.Parse(productsJson);
				IList<JToken> results = productsSearch["data"].Children().ToList();

				IList<ProductModel> searchResults = new List<ProductModel>();
				foreach (JToken result in results)
				{
					// JToken.ToObject is a helper method that uses JsonSerializer internally
					ProductModel searchResult = result.ToObject<ProductModel>();
					products.Add(searchResult);
				}
			}

			return products;
		}
	}
}

//{
//    "code": "200",
//    "data": [
//        {
//            "id": 1,
//            "name": "test",
//            "detail": "test",
//            "quantity": "11",
//            "model": "test1",
//            "price": 100,
//            "image": "http://api.shoppreview.nl/public/storage/imagesboxed-bg.jpg",
//            "status": 1,
//            "category_name": "test",
//            "category_id": 1
//        }
//    ]
//}
