using System;
using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.ObjectMapper;
using CompleetKassa.Database.Services;
using CompleetKassa.Log;
using CompleetKassa.Log.Core;
using CompleetKassa.Models;
using Microsoft.EntityFrameworkCore;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace CompleetKassa.Database.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            #region SQL Server
            //var dbConnection = ConfigurationManager.ConnectionStrings["AppDbConnection"].ConnectionString;
            //container.RegisterType<IDatabaseConnection, DefaultDatabaseConnection>(new InjectionConstructor(dbConnection));
            //var options = new DbContextOptionsBuilder<AppDbContext>()
            //.UseSqlServer(dbConnection)
            //.Options;
            #endregion SQL Server

            #region SQLite
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=CompleetKassa.db3;").Options;
            #endregion SQLite

            //container.RegisterType<ILogger>(new InjectionFactory(l => LogManager.GetCurrentClassLogger()));
            //container.RegisterType<ILogger>(LogHelper.GetLogger<NLog>(LogManager.GetCurrentClassLogger()));
            container.RegisterType<AppDbContext>(new TransientLifetimeManager(), new InjectionConstructor(options));

            container.RegisterType<ObjectMapperProvider>(new TransientLifetimeManager());
            container.RegisterInstance(container.Resolve<ObjectMapperProvider>().Mapper);

            container.RegisterType<IAppUser, AppUser>(new InjectionConstructor(1, "LoggedUser"));
            container.RegisterType<ILogger, Logger>(new InjectionConstructor());
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IRoleService, RoleService>();
            container.RegisterType<IResourceService, ResourceService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IProductService, ProductService>();

            //UserTest(container).Wait();
            //CategoryTest(container).Wait();
            //ProductTest(container).Wait();
            //ProductWithCategoryTest(container).Wait();
            UserWithRolesAndResourcesTest(container).Wait();
        }

        private static async Task ProductTest(IUnityContainer container)
        {
            IProductService repo = container.Resolve<IProductService>();

            //var allProductsWithCategory = await repo.GetProductsWithCategoryAsync();

            //var result = await repo.GetByIDWithCategoryAsync(2);

            var newProduct = new ProductModel
            {
                Name = "Product-" + DateTime.Now.ToString(),
                CategoryID = 0,
                Status = 1
            };

            await repo.AddProductAsync(newProduct);

            await repo.GetProductByIDAsync(newProduct.ID);
        }

        private static async Task ProductWithCategoryTest(IUnityContainer container)
        {
            ICategoryService categoryService = container.Resolve<ICategoryService>();
            IProductService repo = container.Resolve<IProductService>();

            // Category with no parent
            var newCategory = new CategoryModel
            {
                Name = "Category-" + DateTime.Now.ToString(),
                Status = 0,
                Parent = 0
            };

            await categoryService.AddCategoryAsync(newCategory);

            var newProduct = new ProductModel
            {
                Name = "Product-" + DateTime.Now.ToString(),
                CategoryID = 1,
                Status = 1
            };

            await repo.AddProductAsync(newProduct);
        }

        private static async Task CategoryTest(IUnityContainer container)
        {
            ICategoryService repo = container.Resolve<ICategoryService>();

            // Category with no parent
            var newCategory = new CategoryModel
            {
                Name = "Category-" + DateTime.Now.ToString(),
                Status = 0,
                Parent = 0
            };

            await repo.AddCategoryAsync(newCategory);

            // Category with Parent
            var anotherCategory = new CategoryModel
            {
                Name = "Category-" + DateTime.Now.ToString(),
                Status = 0,
                Parent = 1
            };

            await repo.AddCategoryAsync(anotherCategory);

        }

        private static async Task UserTest(IUnityContainer container)
        {
            IUserService repo = container.Resolve<IUserService>();

            var newUser = new UserModel
            {
                FirstName = "User-" + DateTime.Now.ToString(),
                LastName = "Last Name"
            };


            await repo.AddUserAsync(newUser);

            var updateUser = repo.GetUserByIDAsync(1);
            updateUser.Result.Model.FirstName = "Modified First Name";

            await repo.UpdateUserAsync(updateUser.Result.Model);

            var list = repo.GetUsersAsync().Result.Model.ToList();


            System.Console.WriteLine(list.Count);
        }

        private static async Task UserWithRolesAndResourcesTest(IUnityContainer container)
        {
            IUserService userService = container.Resolve<IUserService>();
            IRoleService roleService = container.Resolve<IRoleService>();
            IResourceService resourceService = container.Resolve<IResourceService>();

            // About this test
            // 1. Create User
            // 2. Create Roles
            // 3. Create Resources
            // 4. Create Role <-> Resources
            // 5. Create User <-> Role

            // 1. Create User
            var newUser = new UserModel
            {
                FirstName = "User-" + DateTime.Now.ToString(),
                LastName = "Last Name",
                UserName = "User Name",
                Password = "Password"
            };

            await userService.AddUserAsync(newUser);

            #region "Roles"
            // 2. Create Roles
            var role1 = new RoleModel
            {
                Name = "Role 1",
                Description = "Role 1 Description"
            };

            await roleService.AddRoleAsync(role1);

            var role2 = new RoleModel
            {
                Name = "Role 2",
                Description = "Role 2 Description"
            };

            await roleService.AddRoleAsync(role2);
            #endregion "Roles"

            #region "Resources"
            // 3. Create Resources
            var resource1 = new ResourceModel
            {
                Name = "Resource 1",
                Description = "Resource 1 Description"
            };

            await resourceService.AddResourceAsync(resource1);

            var resource2 = new ResourceModel
            {
                Name = "Resource 2",
                Description = "Resource 2 Description"
            };

            await resourceService.AddResourceAsync(resource2);

            var resource3 = new ResourceModel
            {
                Name = "Resource 3",
                Description = "Resource 3 Description"
            };

            await resourceService.AddResourceAsync(resource3);

            var resource4 = new ResourceModel
            {
                Name = "Resource 4",
                Description = "Resource 4 Description"
            };

            await resourceService.AddResourceAsync(resource4);
            #endregion "Resources"

            #region Role Resources
            // 4. Create Role <-> Resources
            

            #endregion Role Resources

        }
    }
}