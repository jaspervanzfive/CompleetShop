using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Services;
using CompleetKassa.Database.Repositories;
using Microsoft.Extensions.Logging;

namespace CompleetKassa.Database.Services
{
	public abstract class BaseService : IService
	{
		protected ILogger Logger;
		protected IAppUser UserInfo;
		protected bool Disposed;
		protected readonly AppDbContext DbContext;

		protected IUserRepository m_userRepository;

		public BaseService(ILogger logger, IAppUser userInfo, AppDbContext dbContext)
		{
			Logger = logger;
			UserInfo = userInfo;
			DbContext = dbContext;
		}

		public void Dispose()
		{
			if (!Disposed)
			{
				DbContext?.Dispose();

				Disposed = true;
			}
		}

		protected string CreateInvokedMethodLog(string methodName)
		{
			return string.Format("{0} has been invoked", methodName);
		}

		protected IUserRepository UserRepository => m_userRepository ?? (m_userRepository = new UserRepository(UserInfo, this.DbContext));
	}
}
