using CompleetKassa.DataTypes.Enumerations;
using System.Windows.Input;

namespace CompleetKassa.Models
{
    public class ProductModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
		public int Quantity { get; set; }
		public string Model { get; set; }
		public decimal Price { get; set; }
		public string Image { get; set; }
		public int Status { get; set; }

		public string Category { get; set; }
		public string SubCategory { get; set; }
	}
}
