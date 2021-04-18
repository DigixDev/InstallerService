using System;
using System.Threading;
using System.Windows;
using InstallerApp.ViewModels;
using Shared.Controls;
using Shared.Core;
using Shared.Remoting;
using Shared.Remoting.Interfaces;

namespace InstallerApp.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx
    {
        private static IServer _server;

        public MainWindow()
        {
            InitializeComponent();

            _server=new Shared.Remoting.TCP.Server();
            _server.Init(OnMessageReceived);

            SizeChanged += (s, e) => RelocateTheWindow();
            StateChanged += MainWindow_StateChanged;
            Closing += (s, e) => _server.Dispose();
        }

        private void OnMessageReceived(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var temp = msg.Split(new char[] {':'});
                switch (temp[0])
                {
                    case GlobalData.CMD_UPDATING:
                        _server.Dispose();
                        Application.Current.Shutdown();
                        break;
                    case GlobalData.CMD_START:
                        ProgressBorder.Visibility = Visibility.Visible;
                        Visibility = Visibility.Visible;
                        break;
                    case GlobalData.CMD_DOWNLOADING:
                        TitleTextBox.Text = temp[1] + " (Downloading...)";
                        AppProgressBar.Value = Convert.ToInt16(temp[2]);
                        break;
                    case GlobalData.CMD_INSTALLING:
                        TitleTextBox.Text = temp[1] + " (Installing...)";
                        AppProgressBar.Value = 100;
                        break;
                    case GlobalData.CMD_STOP:
                        ProgressBorder.Visibility = Visibility.Collapsed;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ((MainViewModel) DataContext).ReadPackFromRemote();
                        });
                        break;
                }
            });
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
                UpdateLayout();
        }

        private void RelocateTheWindow()
        {
            Left = SystemParameters.FullPrimaryScreenWidth - ActualWidth;
            Top = SystemParameters.FullPrimaryScreenHeight - ActualHeight;
        }
    }
}