using System;
using System.Threading;
using System.Windows;
using InstallerApp.ViewModels;
using Shared.Controls;
using Shared.Core;
using Shared.Remoting;

namespace InstallerApp.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx
    {
        private static Shared.Remoting.Server _server;

        public MainWindow()
        {
            InitializeComponent();

            _server=new Server(OnMessageReceived);
            SizeChanged += (s, e) => RelocateTheWindow();
            StateChanged += MainWindow_StateChanged;
        }

        private void OnMessageReceived(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var temp = msg.Split(new char[] {':'});
                switch (temp[0])
                {
                    case GlobalData.CMD_START:
                        ProgressBorder.Visibility = Visibility.Visible;
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
                        ((MainViewModel) DataContext).ReadPackFromSetting();
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