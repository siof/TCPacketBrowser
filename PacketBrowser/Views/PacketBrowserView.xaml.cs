using PacketBrowser.ViewModels;
using System.Windows.Controls;

namespace PacketBrowser.Views
{
    /// <summary>
    /// Interaction logic for PacketBrowserView.xaml
    /// </summary>
    public partial class PacketBrowserView : UserControl
    {
        public PacketBrowserView()
        {
            InitializeComponent();
            DataContext = new PacketBrowserViewModel();
        }
    }
}
