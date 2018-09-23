using System.Windows;
using System.Windows.Controls;
using CompleetKassa.Database.Context;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.ObjectMapper;
using CompleetKassa.Database.Services;
using CompleetKassa.Log;
using CompleetKassa.Log.Core;
using CompleetKassa.Module.Customer;
using CompleetKassa.Module.UserManagement;
using CompleetKassa.Module.ProductManagement;
using CompleetKassa.Modules.Sales;
using CompleetKassa.RegionAdapters;
using CompleetKassa.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;

namespace CompleetKassa
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();

            mappings.RegisterMapping(typeof(StackPanel), Container.TryResolve<StackPanelRegionAdapter>());

            return mappings;
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog)ModuleCatalog;


            catalog.AddModule(typeof(SalesModule));
            catalog.AddModule(typeof(ProductModule));
            catalog.AddModule(typeof(CustomersModule));
            catalog.AddModule(typeof(UserManagementModule));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            #region SQLite
            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=CompleetKassa.db3;").Options;
            #endregion SQLite

            Container.RegisterType<AppDbContext>(new TransientLifetimeManager(), new InjectionConstructor(options));

            Container.RegisterType<ObjectMapperProvider>(new TransientLifetimeManager());
            Container.RegisterInstance(Container.Resolve<ObjectMapperProvider>().Mapper);
            Container.RegisterType<IAppUser, AppUser>(new InjectionConstructor(1, "LoggedUser"));
            Container.RegisterType<ILogger, Logger>(new InjectionConstructor());

            Container.RegisterType<IProductService, ProductService>();
            Container.RegisterType<ICategoryService, CategoryService>();

            Container.RegisterType<IAccountService, AccountService>();
        }
    }
}
