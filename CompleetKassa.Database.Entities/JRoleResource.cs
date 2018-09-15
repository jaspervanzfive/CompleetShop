using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
	public class JRoleResource : IEntity
	{
		public int RoleID { get; set; }
		public Role Role { get; set; }

		public int ResourceID { get; set; }
		public Resource Resource { get; set; }
	}
}
