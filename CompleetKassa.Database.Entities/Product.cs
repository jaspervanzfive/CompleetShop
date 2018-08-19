using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
	public class Product : IAuditableEntity
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Detail { get; set; }
		public int Quantity { get; set; }
		public string Model { get; set; }
		public decimal Price { get; set; }
		public string Image { get; set; }
		public int Status { get; set; }


		public string CategoryName { get; set; }
		public int CategoryID { get; set; }
		public string CreationUser { get; set; }
		public DateTime? CreationDateTime { get; set; }
		public string LastUpdateUser { get; set; }
		public DateTime? LastUpdateDateTime { get; set; }
		public byte[] Timestamp { get; set; }


		//http://api.shoppreview.nl/public/api/products
		//		Accept: application/json
		//Authorization: Bearer a3e9ad5d90d3098d3c8716687bd45180ed67b7f2675f90fbf478c0f51796
		//Authorization: Bearer a3e9ad5d90d3098d3c8716687bd45180ed67b7f2675f90fbf478c0f51796
		//"id": 1,
		//      "name": "test",
		//      "detail": "test",
		//      "quantity": "11",
		//      "model": "test1",
		//      "price": 100,
		//      "image": "http://api.shoppreview.nl/public/storage/imagesboxed-bg.jpg",
		//      "status": 1,
		//      "category_name": "test",
		//      "category_id": 1
	}
}
