using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using InstallerService.UI.Views;
using Shared.Core;
using Application = System.Windows.Application;

namespace InstallerService
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static NotifyIcon _notifyIcon;
        public static string[] Args;

        public static bool Exit { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            CheckForSingleInstance();

            Args = e.Args;

            base.OnStartup(e);

            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = InstallerService.UI.Properties.Resources.Installer;
            _notifyIcon.Click += _notifyIcon_Click;
            _notifyIcon.Visible = true;
            CreateContextMenu();
            ToggleShowWindow();
        }

        private void CheckForSingleInstance()
        {
            _ = new Mutex(false, GlobalData.UUID, out var created);
            if (created == false)
                Current.Shutdown();
        }

        private void _notifyIcon_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs) e).Button == MouseButtons.Left)
                ToggleShowWindow();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (Exit == false)
            {
                e.Cancel = true;
                ToggleShowWindow();
            }
        }

        private void CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("Show List", null, (s, e) => ToggleShowWindow()));
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) =>
            {
                Exit = true;
                Current.Shutdown();
            }));

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        public static void ToggleShowWindow()
        {
            if (Current.MainWindow.IsVisible)
            {
                Current.MainWindow.Hide();
                _notifyIcon.ContextMenuStrip.Items[0].Text = "Show Main Window";
                //_notifyIcon.Visible = true;
            }
            else
            {
                Current.MainWindow.Show();
                _notifyIcon.ContextMenuStrip.Items[0].Text = "Hide Main Window";
                //_notifyIcon.Visible = false;
            }
        }

        private void ShowWindow()
        {
            Current.MainWindow.WindowState = WindowState.Normal;
        }
    }
}