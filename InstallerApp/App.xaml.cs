using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using InstallerApp.Views;
using Shared.Core;
using Shared.Remoting;
using Application = System.Windows.Application;

namespace InstallerApp
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static NotifyIcon _notifyIcon;
        private static string[] Args;
        private static Mutex _mutex;

        public static bool UserExit { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (IsSingleInstance())
            {
                Args = e.Args;

                MainWindow = new MainWindow();
                MainWindow.Closing += MainWindow_Closing;

                _notifyIcon = new NotifyIcon
                {
                    Icon = InstallerApp.Properties.Resources.Installer
                };

                _notifyIcon.Click += NotifyIcon_Click;
                _notifyIcon.Visible = true;

                CreateContextMenu();
                ToggleShowWindow();
            }
        }

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