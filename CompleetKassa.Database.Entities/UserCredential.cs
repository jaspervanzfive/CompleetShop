using System;
using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
	public class UserCredential : IAuditableEntity
	{
		public int ID { get; set; }

		public string UserName { get; set; }
		public string Password { get; set; }

		public virtual int? UserID { get; set; }
		public virtual User User { get; set; }

		public string CreationUser { get; set; }

		public DateTime? CreationDateTime { get; set; }

		public string LastUpdateUser { get; set; }

		public DateTime? LastUpdateDateTime { get; set; }

		public DateTime? Timestamp { get; set; }
	}
}