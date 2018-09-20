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

        private readonly AppDbContext _dbContext;
        private readonly ILogger _logger;
        private IMapper _mapper;
        private IAppUser _userInfo;

        private IResourceRepository _resourceRepository;
        private IProductRepository _productRepository;
        private IRoleRepository _roleRepository;
        private ICategoryRepository _categoryRepository;
        private IUserRepository _userRepository;
        private IUserCredentialRepository _userCredentialRepository;
        private IJUserRoleRepository _userRoleRepository;

        protected AppDbContext DbContext
        {
            get
            {
                return _dbContext;
            }
        }

        protected ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        protected IMapper Mapper
        {
            get
            {
                return _mapper;
            }
        }

        protected IAppUser UserInfo
        {
            get
            {
                return _userInfo;
            }
        }

        protected IResourceRepository ResourceRepository
        {
            get
            {
                return _resourceRepository;
            }
        }

        protected IProductRepository ProductRepository
        {
            get
            {
                return _productRepository;
            }
        }

        protected IRoleRepository RoleRepository
        {
            get
            {
                return _roleRepository;
            }
        }

        protected ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository;
            }
        }

        protected IUserRepository UserRepository
        {
            get
            {
                return _userRepository;
            }
        }

        protected IUserCredentialRepository UserCredentialRepository
        {
            get
            {
                return _userCredentialRepository;
            }
        }

        protected IJUserRoleRepository UserRoleRepository
        {
            get
            {
                return _userRoleRepository;
            }
        }

        public BaseService (ILogger logger, IMapper mapper, IAppUser userInfo, AppDbContext dbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _userInfo = userInfo;
            _dbContext = dbContext;

            _productRepository = new ProductRepository (userInfo, _dbContext);
            _resourceRepository = new ResourceRepository (userInfo, _dbContext);
            _roleRepository = new RoleRepository (userInfo, _dbContext);
            _categoryRepository = new CategoryRepository (userInfo, DbContext);
            _userCredentialRepository = new UserCredentialRepository (userInfo, _dbContext);
            _userRoleRepository = new JUserRoleRepository (userInfo, _dbContext);
            _userRepository = new UserRepository (userInfo, _dbContext);
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
