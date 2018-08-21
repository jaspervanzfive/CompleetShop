using System;
using System.Linq;
using System.Threading.Tasks;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.ObjectMapper;
using CompleetKassa.Database.Services;
using CompleetKassa.Models;
using Microsoft.EntityFrameworkCore;
using NLog;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace CompleetKassa.Database.Console
{
	class Program
	{
		static void Main(string[] args)
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

			container.RegisterType<ILogger>(new InjectionFactory(l => LogManager.GetCurrentClassLogger()));
			//container.RegisterType<ILogger>(LogHelper.GetLogger<NLog>(LogManager.GetCurrentClassLogger()));
			container.RegisterType<AppDbContext>(new TransientLifetimeManager(), new InjectionConstructor(options));

			container.RegisterType<ObjectMapperProvider>(new TransientLifetimeManager());
			container.RegisterInstance(container.Resolve<ObjectMapperProvider>().Mapper);

			container.RegisterType<IAppUser, AppUser>(new InjectionConstructor(1, "LoggedUser"));
			//container.RegisterType<ILogger>(new InjectionFactory((c) => null));
			container.RegisterType<IUserService, UserService>();
			container.RegisterType<ICategoryService, CategoryService>();
			container.RegisterType<IProductService, ProductService>();

			//UserTest(container).Wait();
			CategoryTest(container).Wait();
			ProductTest(container).Wait();

			LogManager.Flush();
		}
		static async Task ProductTest(IUnityContainer container)
		{
			IProductService repo = container.Resolve<IProductService>();

			var allProductsWithCategory = await repo.GetProductsWithCategoryAsync();

			//var result = await repo.GetByIDWithCategoryAsync(2);
			// new Product
			//var newProduct = new ProductModel
			//{
			//	Name = "Product-" + DateTime.Now.ToString(),
			//	CategoryID = 2,
			//	Status = 1
			//};

			//await repo.AddProductAsync(newProduct);
		}

		static async Task CategoryTest (IUnityContainer container)
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

		static async Task UserTest(IUnityContainer container)
		{
			IUserService repo = container.Resolve<IUserService>();

			var newUser = new UserModel
			{
				FirstName = "User-" + DateTime.Now.ToString(),
				LastName = "Last Name"
			};


			await repo.AddUserAsync(newUser);

			var updateUser = repo.GetUsersByIDAsync(1);
			updateUser.Result.Model.FirstName = "Modified First Name";

			await repo.UpdateUserAsync(updateUser.Result.Model);

			var list = repo.GetUsersAsync().Result.Model.ToList();


			System.Console.WriteLine(list.Count);
		}
	}
}