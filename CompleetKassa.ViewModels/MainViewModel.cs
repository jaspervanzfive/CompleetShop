using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;

namespace CompleetKassa.ViewModels
{
    public class MainViewModel : PropertyChangedViewModel
    {
        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        public MainViewModel()
        {
            this.CreateMenuItems();
        }

        public void CreateMenuItems()
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Shopping},
                    Label = "Shoes",
                    ToolTip = "Shoes View",
                    Tag = new ShoesViewModel()
                },
                 new HamburgerMenuIconItem()
                {
                    Icon = new PackIconModern() {Kind = PackIconModernKind.Eye},
                    Label = "UV Filter",
                    ToolTip = "UV Filter View",
                    Tag = new UvFilterViewModel()
                },
                 new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.EarHearing},
                    Label = "Accessories",
                    ToolTip = "Accessories View",
                    Tag = new AccessoriesViewModel(),
                    IsEnabled = false
                },
            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Help},
                    Label = "About",
                    ToolTip = "Some help.",
                    Tag = new AboutViewModel()
                }
            };
        }

        public HamburgerMenuItemCollection MenuItems
        {
            get { return _menuItems; }
            set
            {
                if (Equals(value, _menuItems)) return;
                _menuItems = value;
                OnPropertyChanged();
            }
        }

        public HamburgerMenuItemCollection MenuOptionItems
        {
            get { return _menuOptionItems; }
            set
            {
                if (Equals(value, _menuOptionItems)) return;
                _menuOptionItems = value;
                OnPropertyChanged();
            }
        }
    }
}
