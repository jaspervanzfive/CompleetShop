using System;
using CompleetShop.Database.Core.Contracts;

namespace CompleetShop.Database.EntityLayer
{
	public class User : IAuditableEntity
	{
		public User ()
		{
		}

		public User (int userID)
		{
			ID = userID;
		}

		public int ID { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public string LastName { get; set; }

		public DateTime? BirthDate { get; set; }

		public string CreationUser { get; set; }

		public DateTime? CreationDateTime { get; set; }

		public string LastUpdateUser { get; set; }

		public DateTime? LastUpdateDateTime { get; set; }

		public byte[] Timestamp { get; set; }
	}
}
