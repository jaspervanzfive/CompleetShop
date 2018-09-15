namespace CompleetKassa.Models
{
	public class ProductModel
	{
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public string Image { get; set; }
        public bool Favorite { get; set; }
        public int Quantity { get; set; }
        public int MinimumStock { get; set; }
        public int Status { get; set; }

        public int CategoryID { get; set; }
		public int SubCategoryID { get; set; }

		public string Category { get; set; }
		public string SubCategory { get; set; }
		public string ImageAbsolutePath { get; set; }
	}
}
