using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using InstallerApp.ViewModels;
using InstallerApp.Views;
using Serilog;
using Shared.Core;
using Shared.Remoting;
using Shared.Remoting.Interfaces;
using Application = System.Windows.Application;

namespace InstallerApp
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static NotifyIcon _notifyIcon;
        public static string[] Args;
        private static Mutex _mutex;
        private static IServer _server;

        public static bool UserExit { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (IsSingleInstance())
            {
                Args = e.Args;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("c:\\log\\AppInstaller.txt")
                    .CreateLogger();

                MainWindow = new MainWindow();
                MainWindow.Closing += MainWindow_Closing;

                _notifyIcon = new NotifyIcon
                {
                    Icon = InstallerApp.Properties.Resources.Installer
                };

                _notifyIcon.Click += NotifyIcon_Click;
                _notifyIcon.Visible = true;

                var view = (MainWindow) Application.Current.MainWindow;

                _server = new Shared.Remoting.TCP.Server();
                _server.Init(view.OnMessageReceived);

                CreateContextMenu();
                ToggleShowWindow();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _server?.Dispose();
        }

        //public void OnMessageReceived(string msg)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        var temp = msg.Split(new char[] { ':' });
        //        var view = Application.Current.MainWindow as MainWindow;

        //        switch (temp[0])
        //        {
        //            case GlobalData.CMD_UPDATING:
        //                Application.Current.Shutdown();
        //                break;
        //            case GlobalData.CMD_START:
        //                view.ProgressBorder.Visibility = Visibility.Visible;
        //                view.Visibility = Visibility.Visible;
        //                break;
        //            case GlobalData.CMD_DOWNLOADING:
        //                view.TitleTextBox.Text = temp[1] + " (Downloading...)";
        //                view.AppProgressBar.Value = Convert.ToInt16(temp[2]);
        //                break;
        //            case GlobalData.CMD_INSTALLING:
        //                view.TitleTextBox.Text = temp[1] + " (Installing...)";
        //                view.AppProgressBar.Value = 100;
        //                break;
        //            case GlobalData.CMD_STOP:
        //                view.ProgressBorder.Visibility = Visibility.Collapsed;
        //                Application.Current.Dispatcher.Invoke(() =>
        //                {
        //                    ((MainViewModel)view.DataContext).ReadPackFromSetting();
        //                });
        //                break;
        //        }
        //    });
        //}

        private bool IsSingleInstance()
        {
            _mutex = new Mutex(false, "InstallerServicxe.InstallerApp", out var isNew);
            if (isNew == false)
                Current.Shutdown();

            return isNew;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs) e).Button == MouseButtons.Left)
                ToggleShowWindow();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (UserExit == false)
            {
                e.Cancel = true;
                ToggleShowWindow();
            }
        }

        private void CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            //contextMenu.Items.Add(new ToolStripMenuItem("Show List", null, (s, e) => ToggleShowWindow()));
            //contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) =>
            {
                UserExit = true;
                Current.Shutdown();
            }));

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        public static void ToggleShowWindow()
        {
            if (Current.MainWindow.IsVisible)
                Current.MainWindow.Hide();
            else
                Current.MainWindow.Show();
        }
    }
}