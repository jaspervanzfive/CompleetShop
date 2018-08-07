using CompleetKassa.Database.Core.Services;

namespace CompleetKassa.Database.Console
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
