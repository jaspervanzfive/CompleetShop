using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
	public class JUserRole : IEntity
	{
		public int UserId { get; set; }
		public User User { get; set; }

		public int RoleId { get; set; }
		public Role Role { get; set; }
	}
}
