using AutoMapper;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Core.Services;
using CompleetKassa.Database.Repositories;
using CompleetKassa.Log.Core;

namespace CompleetKassa.Database.Services
{
    public abstract class BaseService : IService
    {
        protected bool Disposed;

        protected AppDbContext DbContext { get; }

        protected ILogger Logger { get; }

        protected IMapper Mapper { get; }

        protected IAppUser UserInfo { get; }

        protected IResourceRepository ResourceRepository { get; }

        protected IProductRepository ProductRepository { get; }

        protected IRoleRepository RoleRepository { get; }

        protected ICategoryRepository CategoryRepository { get; }

        protected IUserRepository UserRepository { get; }

        protected IUserCredentialRepository UserCredentialRepository { get; }

        protected IJUserRoleRepository UserRoleRepository { get; }

        protected IJRoleResourceRepository RoleResourceRepository { get; }

        public BaseService (ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
        {
            Logger = logger;
            Mapper = mapper;
            UserInfo = userInfo;
            DbContext = dbContext;

            ProductRepository = new ProductRepository (userInfo, DbContext);
            ResourceRepository = new ResourceRepository (userInfo, DbContext);
            RoleRepository = new RoleRepository (userInfo, DbContext);
            CategoryRepository = new CategoryRepository (userInfo, DbContext);
            UserCredentialRepository = new UserCredentialRepository (userInfo, DbContext);
            UserRoleRepository = new JUserRoleRepository (userInfo, DbContext);
            UserRepository = new UserRepository (userInfo, DbContext);
            RoleResourceRepository = new JRoleResourceRepository(userInfo, DbContext);
        }

        public void Dispose ()
        {
            if (!Disposed) {
                DbContext?.Dispose ();

                Disposed = true;
            }
        }

        protected string CreateInvokedMethodLog (string methodName)
        {
            return string.Format ("{0} has been invoked", methodName);
        }
    }
}
