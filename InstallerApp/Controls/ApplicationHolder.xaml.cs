using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InstallerApp.Annotations;
using Shared.Helpers;
using Shared.Models;

namespace InstallerApp.Controls
{
    /// <summary>
    ///     Interaction logic for ApplicationHolder.xaml
    /// </summary>
    public partial class ApplicationHolder : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty UninstallCommandProperty =
            DependencyProperty.Register("UninstallCommand", typeof(ICommand), typeof(ApplicationHolder),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsReadyProperty = DependencyProperty.Register("IsReady", typeof(bool),
            typeof(ApplicationHolder), new PropertyMetadata(true));

        public static readonly DependencyProperty InstallCommandProperty = DependencyProperty.Register("InstallCommand",
            typeof(ICommand), typeof(ApplicationHolder), new PropertyMetadata(null));

        public static readonly DependencyProperty AppInfoProperty = DependencyProperty.Register("AppInfo",
            typeof(AppInfo), typeof(ApplicationHolder), new PropertyMetadata(null, OnAppInfoChanged));

        private Visibility _downloadVisiblity, _uninstallVisibility;

        public ApplicationHolder()
        {
            InitializeComponent();
        }

        public Visibility UninstallVisibility
        {
            get => _uninstallVisibility;
            set
            {
                if (_uninstallVisibility != value)
                {
                    _uninstallVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public Visibility DownloadVisibility
        {
            get => _downloadVisiblity;
            set
            {
                if (_downloadVisiblity != value)
                {
                    _downloadVisiblity = value;
                    OnPropertyChanged();
                }
            }
        }

        public AppInfo AppInfo
        {
            get => (AppInfo) GetValue(AppInfoProperty);
            set => SetValue(AppInfoProperty, value);
        }

        public ICommand InstallCommand
        {
            get => (ICommand) GetValue(InstallCommandProperty);
            set => SetValue(InstallCommandProperty, value);
        }

        public ICommand UninstallCommand
        {
            get => (ICommand) GetValue(UninstallCommandProperty);
            set => SetValue(UninstallCommandProperty, value);
        }

        public bool IsReady
        {
            get => (bool) GetValue(IsReadyProperty);
            set => SetValue(IsReadyProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnAppInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                ((ApplicationHolder) d).CheckForInstallation();
        }

        private void CheckForInstallation()
        {
            AppInfo.UninstallCommand = RegistryHelper.GetUninstallCommand(AppInfo.Name);

            DownloadVisibility = string.IsNullOrEmpty(AppInfo.UninstallCommand)
                ? Visibility.Visible
                : Visibility.Collapsed;
            UninstallVisibility = string.IsNullOrEmpty(AppInfo.UninstallCommand)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}