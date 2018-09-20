namespace CompleetKassa.Module.ProductManagement.Definitions
{
	public class Enumeration
	{
		public enum Commands
		{
			None = 0,
			AddProduct,
			AddCategory,
			EditProduct,
			EditCategory,
			DeleteProduct,
			DeleteCategory
		}

		public enum TabIndexes
		{
			Product = 0,
			Category = 1
		}
	}
}
