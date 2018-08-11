using System.Windows;
using System.Windows.Input;

namespace CompleetKassa.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Right || e.Key == Key.Left || e.Key == Key.LeftAlt || e.Key == Key.RightAlt)
            {
                e.Handled = true;
            }
          

        }
        
    }
}
