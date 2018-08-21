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
		public string Code { get; set; }
		public string Name { get; set; }
		public string Detail { get; set; }
		public int Quantity { get; set; }
		public string Model { get; set; }
		public decimal Price { get; set; }
		public string Image { get; set; }
		public int Status { get; set; }
		public string CategoryName { get; set; }
		public string SubCategoryName { get; set; }

		// Foreign Key
		public virtual int? CategoryID { get; set; }
		public virtual Category Category { get; set; }
		
		public string CreationUser { get; set; }
		public DateTime? CreationDateTime { get; set; }
		public string LastUpdateUser { get; set; }
		public DateTime? LastUpdateDateTime { get; set; }
		public DateTime? Timestamp { get; set; }
	}
}
