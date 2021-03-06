using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using InstallerService.Views;
using Application = System.Windows.Application;

namespace InstallerService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _exit;

        public static string[] Args;

        protected override void OnStartup(StartupEventArgs e)
        {
            CheckForSingleInstance();

            //ChackForSetup(e.Args);

            Args = e.Args;

            base.OnStartup(e);

            MainWindow=new MainWindow();
            MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = InstallerService.Properties.Resources.Installer;
            _notifyIcon.DoubleClick += _notifyIcon_DoubleClick;
            _notifyIcon.Visible = true;
            CreateContextMenu();
            ToggleShowWindow();
        }

        private void ChackForSetup(string[] args)
        {
            var fullPath = Assembly.GetExecutingAssembly().Location;
            var dir = System.IO.Path.GetDirectoryName(fullPath);

            if (args.Length == 0)
            {
                Shared.Helpers.RegistryHelper.UpdateApplicationPath(fullPath);
                if(Directory.Exists(dir)==false)
                    File.Copy(fullPath, Path.Combine(Shared.Core.GlobalData.APP_FOLDER, Shared.Core.GlobalData.APP_NAME));

            }
            else if(args[0].Equals(Shared.Core.GlobalData.APP_FOLDER,
                StringComparison.InvariantCultureIgnoreCase))
            { 
            }
        }

        private void CheckForSingleInstance()
        {
            var mutext=new Mutex(false, Shared.Core.GlobalData.UUID, out var created);
            if(created==false)
                Current.Shutdown();
        }

        private void _notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ToggleShowWindow();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_exit == false)
            {
                e.Cancel = true;
                ToggleShowWindow();
            }
        }

        private void CreateContextMenu()
        {
            var contextMenu=new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("Show List", null, (s, e) =>ShowWindow()));
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (s, e) => { _exit = true; Application.Current.Shutdown(); }));

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ToggleShowWindow()
        {
            MainWindow.Visibility = MainWindow.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void ShowWindow()
        {
            MainWindow.Visibility = Visibility.Visible;
        }
    }

    
}
