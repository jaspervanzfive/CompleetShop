using CompleetKassa.Module.Product.Commands;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
namespace CompleetKassa.Module.Product.ViewModels
{
    class ProductManagementViewModel : BindableBase
    {

        private string _title = "Product Management";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private IModuleCommands _moduleCommands;
        public IModuleCommands ModuleCommands
        {
            get { return _moduleCommands; }
            set { SetProperty(ref _moduleCommands, value); }
        }

        public ProductManagementViewModel(IUnityContainer container)
        {
            ModuleCommands = container.Resolve<IModuleCommands>();
        }
    }
}
