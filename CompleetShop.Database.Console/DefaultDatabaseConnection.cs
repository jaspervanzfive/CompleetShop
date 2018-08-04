using CompleetShop.Database.Core.Services;

namespace CompleetShop.Database.Console
{
	public class DefaultDatabaseConnection : IDatabaseConnection
	{
		public string NameOrConnectionString { get; private set; }

		public DefaultDatabaseConnection ()
		{
			NameOrConnectionString = "DefaultConnection";
		}

		public DefaultDatabaseConnection (string connectionName)
		{
			NameOrConnectionString = connectionName;
		}
	}
}
