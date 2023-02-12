using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetNugets.Views
{
    /// <summary>
    /// Interaction logic for DownloadedView.xaml
    /// </summary>
    public partial class DownloaderView : UserControl
    {
        public DownloaderView()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService<DownloaderViewModel>();
        }
    }
}
