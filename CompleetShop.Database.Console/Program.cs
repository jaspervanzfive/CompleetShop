using CompleetShop.Database.BusinessLayer.Contracts;
using CompleetShop.Database.BusinessLayer.Services;
using CompleetShop.Database.Core.Contracts;
using CompleetShop.Database.DataLayer;
using CompleetShop.Database.EntityLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace CompleetShop.Database.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            #region MSSQL
            //var dbConnection = ConfigurationManager.ConnectionStrings["AppDbConnection"].ConnectionString;
            //container.RegisterType<IDatabaseConnection, DefaultDatabaseConnection>(new InjectionConstructor(dbConnection));

            //var options = new DbContextOptionsBuilder<AppDbContext>()
            //            .UseSqlServer(dbConnection)
            //            .Options;
            #endregion

            #region SQLite
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=CompleetShop.db;").Options;
            #endregion

            container.RegisterType<AppDbContext>(new TransientLifetimeManager(), new InjectionConstructor(options));
            container.RegisterType<IUserInfo, UserInfo>();
            container.RegisterType<ILogger>(new InjectionFactory((c) => null));
            container.RegisterType<IUserService, UserService>();
            IUserInfo user = container.Resolve<IUserInfo>();

            MainAsync(container).Wait();
        }

        static async Task MainAsync(IUnityContainer container)
        {
            IUserService repo = container.Resolve<IUserService>();

            var newUser = new User
            {
                FirstName = "User-" + DateTime.Now.ToString(),
                BirthDate = DateTime.Now,
                LastName = "Your Last Name"
            };


            await repo.AddUserAsync(newUser);
            //await repo.AddAsync (newUser);

            var updateUser = repo.GetUsersByIDAsync(1);
            updateUser.Result.Model.FirstName = "Aki";

            await repo.UpdateUserAsync(updateUser.Result.Model);

            var list = repo.GetUsersAsync().Result.Model.ToList();


            System.Console.WriteLine(list.Count);
        }
    }
}