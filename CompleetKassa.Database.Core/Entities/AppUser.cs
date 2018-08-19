namespace CompleetKassa.Database.Core.Entities
{
	public class AppUser : IAppUser
	{
		public AppUser(int ID, string name)
		{
			this.ID = ID;
			this.Name = name;
		}

		public int ID { get; set; }

		public string Name { get; set; }
	}
}
