using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompleetShop.Database.Core.Contracts;

namespace CompleetShop.Database.Console
{
	public class UserInfo : IUserInfo
	{
		public UserInfo ()
		{
			ID = 0;
			Name = "Login-User";
		}

		public int ID { get; set; }
		public string Name { get; set; }
	}
}
