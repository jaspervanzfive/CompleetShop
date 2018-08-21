namespace CompleetKassa.Models
{
	public class CategoryModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Detail { get; set; }
		public int Parent { get; set; }
		public string ParentName { get; set; }
		public int Status { get; set; }
		public string Color { get; set; }
	}
}