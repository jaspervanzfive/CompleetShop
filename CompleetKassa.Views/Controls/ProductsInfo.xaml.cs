using CompleetKassa.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CompleetKassa.Views.Controls
{
    /// <summary>
    /// Interaction logic for ProductInfo.xaml
    /// </summary>
    public partial class ProductsInfo : UserControl
    {
        public ProductsInfo()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty ProductsProperty =
                DependencyProperty.Register(
                "Products",
                typeof(ICollectionView),
                typeof(ProductsInfo),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = false,
                    PropertyChangedCallback = ProductsPropertyChanged
                });

        private static void ProductsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ProductsInfo)d;
            control.Products = (ICollectionView)e.NewValue;
        }

        public ICollectionView Products
        {
            get { return GetValue(ProductsProperty) as ICollectionView; }
            set { SetValue(ProductsProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double),
                typeof(ProductsInfo), new PropertyMetadata(-1.0));

        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness),
                typeof(ProductsInfo), new PropertyMetadata(new Thickness(0)));

        #region PurchasedItem
        public static readonly DependencyProperty PurchasedItemProperty =
            DependencyProperty.Register(
                "PurchasedItem",
                typeof(ICommand),
                typeof(ProductsInfo),
                new UIPropertyMetadata(null));
        public ICommand PurchasedItem
        {
            get { return (ICommand)GetValue(PurchasedItemProperty); }
            set { SetValue(PurchasedItemProperty, value); }
        }
        #endregion
    }
}
