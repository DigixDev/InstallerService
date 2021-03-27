using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using InstallerApp.Annotations;
using InstallerApp.ViewModels;
using Shared.Controls;
using Shared.Core;

namespace InstallerApp.Views
{
    /// <summary>
    ///     Interaction logic for GetUrlView.xaml
    /// </summary>
    public partial class SettingView :WindowEx
    {
        public SettingView()
        {
            InitializeComponent();

            SizeChanged += (s, e) =>
            {
                Left = Application.Current.MainWindow.Left - ActualWidth - 3;
                Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.ActualHeight -
                      ActualHeight;
            };

            DataContext = new SettingViewModel(this);
        }
    }
}