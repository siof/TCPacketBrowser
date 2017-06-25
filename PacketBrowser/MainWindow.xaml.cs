using MahApps.Metro.Controls;
using PacketBrowser.ViewModels;

namespace PacketBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            DataContext = new PacketBrowserViewModel();
            InitializeComponent();
        }
    }
}
