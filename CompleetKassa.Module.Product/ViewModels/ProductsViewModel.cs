using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using CompleetKassa.Common;
using CompleetKassa.Database.Services;
using CompleetKassa.Events;
using CompleetKassa.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace CompleetKassa.Modules.Products.ViewModels
{
	public class ProductsViewModel : BindableBase
	{
		#region Fields
		IProductService _productService;
		IEventAggregator _eventAggregator;
		IRegionManager _regionManager;
		#endregion Fields

		#region Property
		public Task Initialization { get; private set; }
		#endregion Property

		#region Command Property
		public DelegateCommand OnFirstCommand { get; private set; }
		public DelegateCommand OnPreviousCommand { get; private set; }
		public DelegateCommand OnNextCommand { get; private set; }
		public DelegateCommand OnLastCommand { get; private set; }
		public DelegateCommand OnAddCommand { get; private set; }
		public DelegateCommand OnSaveCommand { get; private set; }
		public DelegateCommand<ProductModel> OnDeleteCommand { get; private set; }
		public DelegateCommand OnCancelCommand { get; private set; }
		public DelegateCommand OnSelectImageCommand { get; private set; }
		#endregion Command Property

		#region "Bindable Property"
		private string _title;

		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		private bool _newUserFormVisibility;
		public bool NewProductFormVisibility
		{
			get { return _newUserFormVisibility; }
			set { SetProperty(ref _newUserFormVisibility, value); }
		}

		#region New Product Object
		private ProductModel _newProductModel;
		public ProductModel NewProductModel
		{
			get { return _newProductModel; }
			set { SetProperty(ref _newProductModel, value); }
		}

		private string _selectedImagePath;
		public string SelectedImagePath
		{
			get { return _selectedImagePath; }
			set { SetProperty(ref _selectedImagePath, value); }
		}

		private Uri _selectedImageUri;
		public Uri SelectedImageUri
		{
			get { return _selectedImageUri; }
			set { SetProperty(ref _selectedImageUri, value); }
		}

		#endregion New Product Object

		public ICollectionView _ProductListView;
		public ICollectionView ProductListView
		{
			get { return _ProductListView; }
			private set { SetProperty(ref _ProductListView, value); }
		}
		#endregion "Bindable Property"

		public ProductsViewModel(IUnityContainer container)
		{
			_regionManager = container.Resolve<IRegionManager>();
			_productService = container.Resolve<IProductService>();
			_eventAggregator = container.Resolve<IEventAggregator>();

			_newProductModel = new ProductModel();
			_title = "-Product Management Title-";
			NewProductFormVisibility = false;

			OnFirstCommand = new DelegateCommand(FirstCommandHandler);
			OnPreviousCommand = new DelegateCommand(PreviousCommandHandler);
			OnNextCommand = new DelegateCommand(NextCommandHandler);
			OnLastCommand = new DelegateCommand(LastCommandHandler);
			OnAddCommand = new DelegateCommand(AddCommandHandler);
			OnSaveCommand = new DelegateCommand(SaveCommandHandler);
			OnDeleteCommand = new DelegateCommand<ProductModel>(DeleteCommandHandler);
			OnCancelCommand = new DelegateCommand(CancelCommandHandler);
			OnSelectImageCommand = new DelegateCommand(SelectImageHandler);

			Initialization = InitializeAsync();
			ProductListView.MoveCurrentToFirst();
		}

		private void SelectImageHandler()
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				openFileDialog.Filter = "Image Files(*.PNG;*.JPG)|*.PNG;*.JPG|All files (*.*)|*.*";
				openFileDialog.FilterIndex = 1;
				openFileDialog.RestoreDirectory = true;

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					var targetPath = Path.Combine(Path.GetTempPath(), ApplicationFolders.ProductsTmpFolder);

					// Create a new target folder, if necessary.
					if (!Directory.Exists(targetPath))
					{
						Directory.CreateDirectory(targetPath);
					}

					SelectedImagePath = Path.Combine(ApplicationFolders.Products, Path.GetFileName(openFileDialog.FileName));
					var destFile = Path.Combine (targetPath, Path.GetFileName(openFileDialog.FileName));
					// To copy a file to another location and 
					// overwrite the destination file if it already exists.
					File.Copy(openFileDialog.FileName, destFile, true);

					SelectedImageUri = new Uri(destFile);
				}
			}
		}

		private async Task InitializeAsync()
		{
			var result = await _productService.GetProductsAsync();
			if (result.DidError == false)
			{
				ProductListView = CollectionViewSource.GetDefaultView(result.Model.ToList());
			}
		}

		private void LastCommandHandler()
		{
			ProductListView.MoveCurrentToLast();
		}

		private void PreviousCommandHandler()
		{
			ProductListView.MoveCurrentToPrevious();

			if (ProductListView.IsCurrentBeforeFirst == true)
			{
				ProductListView.MoveCurrentToFirst();
			}
		}

		private void NextCommandHandler()
		{

			ProductListView.MoveCurrentToNext();

			if (ProductListView.IsCurrentAfterLast == true)
			{
				ProductListView.MoveCurrentToLast();
			}
		}

		private void FirstCommandHandler()
		{
			ProductListView.MoveCurrentToFirst();
		}

		private void AddCommandHandler()
		{
			NewProductFormVisibility = true;
			NewProductModel = new ProductModel();
		}

		private async void SaveCommandHandler()
		{
			NewProductFormVisibility = false;

			NewProductModel.Image = SelectedImagePath;

			var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ApplicationFolders.Products);
			// Create a new target folder, if necessary.
			if (!Directory.Exists(targetPath))
			{
				Directory.CreateDirectory(targetPath);
			}

			var destFile = Path.Combine(targetPath, Path.GetFileName(SelectedImageUri.ToString()));
			// To copy a file to another location and 
			// overwrite the destination file if it already exists.
			File.Copy(SelectedImageUri.LocalPath, destFile, true);

			var result = await _productService.AddProductAsync(NewProductModel);
			await InitializeAsync();
		}

		private async void DeleteCommandHandler(ProductModel user)
		{
			if (user != null)
			{
				await _productService.RemoveProductAsync(user.ID);
				await InitializeAsync();
			}
		}

		private void CancelCommandHandler()
		{
			NewProductFormVisibility = false;
		}
	}
}
