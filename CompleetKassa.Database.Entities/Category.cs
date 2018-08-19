using System;
using System.Collections.Generic;
using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
	public class Category : IAuditableEntity
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Detail { get; set; }
		public int Status { get; set; }
		// Links
		public virtual int? ParentCategoryID { get; set; }
		public virtual Category ParentCategory { get; set; }

		// For Audit
		public string CreationUser { get; set; }
		public DateTime? CreationDateTime { get; set; }
		public string LastUpdateUser { get; set; }
		public DateTime? LastUpdateDateTime { get; set; }
		public DateTime? Timestamp { get; set; }

		//http://api.shoppreview.nl/public/api/categories
		//Accept: application/json
		//Authorization: Bearer a3e9ad5d90d3098d3c8716687bd45180ed67b7f2675f90fbf478c0f51796
		//   "id": 1,
		//   "name": "test",
		//   "detail": "test",
		//   "parent": 0,
		//   "status": 1
	}
}
