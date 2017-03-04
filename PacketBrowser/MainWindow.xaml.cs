using PacketBrowser.ViewModels;
using System.Windows;

namespace PacketBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new PacketBrowserViewModel();
            InitializeComponent();
        }
    }
}
