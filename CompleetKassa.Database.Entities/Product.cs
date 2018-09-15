using System;
using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
    public class Product : AuditableBaseEntity, IAuditableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public int MinimumStock { get; set; }
        public int Status { get; set; }
        public bool Favorite { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        // Foreign Key
        public virtual int? CategoryID { get; set; }
        public virtual Category Category { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
