using System.Windows;
using InstallerService.UI.ViewModels;
using Shared.Models;

namespace InstallerService.UI.Views
{
    /// <summary>
    ///     Interaction logic for DownloadView.xaml
    /// </summary>
    public partial class DownloadView
    {
        private readonly DownloadViewModel _vmDownload;

        public DownloadView(AppInfo appInfo)
        {
            InitializeComponent();
            DataContext = _vmDownload = new DownloadViewModel(this, appInfo);
            SizeChanged += DownloadView_SizeChanged;
        }

        private void DownloadView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = Application.Current.MainWindow.Left - ActualWidth - 3;
            Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.ActualHeight -
                  ActualHeight;
        }

        protected override void Start()
        {
            _vmDownload.Start();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}