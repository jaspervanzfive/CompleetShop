using AutoMapper;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Services;
using NLog;

namespace CompleetKassa.Database.Services
{
	public abstract class BaseService : IService
	{
		protected ILogger Logger;
		protected IMapper Mapper;
		protected IAppUser UserInfo;
		protected bool Disposed;
		protected readonly AppDbContext DbContext;

		public BaseService(ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
		{
			Logger = logger;
			Mapper = mapper;
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
	}
}
