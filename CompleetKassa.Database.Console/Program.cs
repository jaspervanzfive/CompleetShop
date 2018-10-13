using System;
using System.Collections.Generic;
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
            container.RegisterType<IAccountService, AccountService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<IAccountService, AccountService>();

            //UserTest(container).Wait();
            //CategoryTest(container).Wait();
            //ProductTest(container).Wait();
            //ProductWithCategoryTest(container).Wait();
            //UserWithRolesAndResourcesTest(container).Wait();
            //AccountServiceTest(container).Wait();
            GetUserEagerTest(container).Wait();

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
            IAccountService repo = container.Resolve<IAccountService>();

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
            IAccountService accountService = container.Resolve<IAccountService>();

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

            await accountService.AddUserAsync(newUser);

            #region "Roles"
            // 2. Create Roles
            var role1 = new RoleModel
            {
                Name = "Role 1",
                Description = "Role 1 Description"
            };

            await accountService.AddRoleAsync(role1);

            var role2 = new RoleModel
            {
                Name = "Role 2",
                Description = "Role 2 Description"
            };

            await accountService.AddRoleAsync(role2);
            #endregion "Roles"

            #region "Resources"
            // 3. Create Resources
            var resource1 = new ResourceModel
            {
                Name = "Resource 1",
                Description = "Resource 1 Description"
            };

            await accountService.AddResourceAsync(resource1);

            var resource2 = new ResourceModel
            {
                Name = "Resource 2",
                Description = "Resource 2 Description"
            };

            await accountService.AddResourceAsync(resource2);

            var resource3 = new ResourceModel
            {
                Name = "Resource 3",
                Description = "Resource 3 Description"
            };

            await accountService.AddResourceAsync(resource3);

            var resource4 = new ResourceModel
            {
                Name = "Resource 4",
                Description = "Resource 4 Description"
            };

            await accountService.AddResourceAsync(resource4);
            #endregion "Resources"

            #region Role Resources
            // 4. Create Role <-> Resources
            

            #endregion Role Resources

        }

        private static async Task AccountServiceTest(IUnityContainer container)
        {
            IAccountService accountService = container.Resolve<IAccountService>();

            #region "Roles"
            // Create Roles
            var role1 = new RoleModel
            {
                Name = "Role 1",
                Description = "Role 1 Description"
            };

            var userRole1 = await accountService.AddRoleAsync(role1);

            var role2 = new RoleModel
            {
                Name = "Role 2",
                Description = "Role 2 Description"
            };

            var userRole2 = await accountService.AddRoleAsync(role2);

            var role3 = new RoleModel
            {
                Name = "Role 3",
                Description = "Role 3 Description"
            };

            var userRole3 = await accountService.AddRoleAsync(role3);
            #endregion "Roles"

            #region "Resources"
            // Create Resources
            var resource1 = new ResourceModel
            {
                Name = "Resource 1",
                Description = "Resource 1 Description"
            };

            var userResource1 = await accountService.AddResourceAsync(resource1);

            var resource2 = new ResourceModel
            {
                Name = "Resource 2",
                Description = "Resource 2 Description"
            };

            var userResource2 = await accountService.AddResourceAsync(resource2);

            var resource3 = new ResourceModel
            {
                Name = "Resource 3",
                Description = "Resource 3 Description"
            };

            var userResource3 = await accountService.AddResourceAsync(resource3);

            var resource4 = new ResourceModel
            {
                Name = "Resource 4",
                Description = "Resource 4 Description"
            };

            var userResource4 = await accountService.AddResourceAsync(resource4);
            #endregion "Resources"

            #region Role Resources
            await accountService.AddRoleResourceAsync(userRole1.Model.ID, userResource1.Model.ID);
            await accountService.AddRoleResourceAsync(userRole1.Model.ID, userResource2.Model.ID);
            await accountService.AddRoleResourceAsync(userRole1.Model.ID, userResource3.Model.ID);
            await accountService.AddRoleResourceAsync(userRole2.Model.ID, userResource1.Model.ID);
            await accountService.AddRoleResourceAsync(userRole2.Model.ID, userResource4.Model.ID);
            await accountService.AddRoleResourceAsync(userRole3.Model.ID, userResource1.Model.ID);
            #endregion Role Resources

            // Create User
            var newUser = new UserModel
            {
                FirstName = "User-" + DateTime.Now.ToString(),
                LastName = "Last Name",
                UserName = "User Name",
                Password = "Password",
                Roles = new List<RoleModel> { userRole1.Model, userRole2.Model }
            };

            var response = await accountService.AddUserAccountAsync(newUser);

            var userResponse = await accountService.AddUserRoleAsync(response.Model.ID, userRole3.Model.ID);
        }

        private static async Task GetUserEagerTest(IUnityContainer container)
        {
            IAccountService accountService = container.Resolve<IAccountService>();

            var response = await accountService.GetFirstOrDefaultAsync(1);
        }
    }
}