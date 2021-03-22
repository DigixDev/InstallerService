using System;
using System.Windows;
using Shared.Controls;

namespace InstallerService.UI.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();
            SizeChanged += (s, e) => RelocateTheWindow();
            StateChanged += MainWindow_StateChanged;
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