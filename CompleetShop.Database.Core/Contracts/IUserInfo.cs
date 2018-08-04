using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompleetShop.Database.Core.Contracts
{
	public interface IUserInfo
	{
		int ID { get; set; }
		string Name { get; set; }
	}
}
