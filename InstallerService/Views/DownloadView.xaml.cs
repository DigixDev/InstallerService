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
using System.Windows.Shapes;
using InstallerService.ViewModels;
using Shared.Controls;
using Shared.Models;

namespace InstallerService.Views
{
    /// <summary>
    /// Interaction logic for DownloadView.xaml
    /// </summary>
    public partial class DownloadView : WindowEx
    {
        public DownloadView(AppInfo appInfo)
        {
            InitializeComponent();
            DataContext = new DownloadViewModel(this, appInfo);
            SizeChanged += DownloadView_SizeChanged;
        }

        private void DownloadView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Left = Application.Current.MainWindow.Left - this.ActualWidth - 3;
            this.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.ActualHeight -
                       this.ActualHeight;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
