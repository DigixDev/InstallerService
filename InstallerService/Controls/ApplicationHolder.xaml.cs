using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InstallerService.Annotations;
using Shared.Models;

namespace InstallerService.Controls
{
    /// <summary>
    /// Interaction logic for ApplicationHolder.xaml
    /// </summary>
    public partial class ApplicationHolder : UserControl, INotifyPropertyChanged
    {

        private Visibility _downloadVisiblity, _uninstallVisibility;

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
            get { return (AppInfo)GetValue(AppInfoProperty); }
            set { SetValue(AppInfoProperty, value); }
        }

        public ICommand DownloadCommand
        {
            get { return (ICommand)GetValue(DownloadCommandProperty); }
            set { SetValue(DownloadCommandProperty, value); }
        }

        public bool IsReady
        {
            get { return (bool)GetValue(IsReadyProperty); }
            set { SetValue(IsReadyProperty, value); }
        }
        
        public string UninstallText
        {
            get { return (string)GetValue(UninstallTextProperty); }
            set { SetValue(UninstallTextProperty, value); }
        }

        public ICommand UninstallCommand
        {
            get { return (ICommand)GetValue(UninstallCommandProperty); }
            set { SetValue(UninstallCommandProperty, value); }
        }

        public static readonly DependencyProperty UninstallCommandProperty = DependencyProperty.Register("UninstallCommand", typeof(ICommand), typeof(ApplicationHolder), new PropertyMetadata(null));
        public static readonly DependencyProperty UninstallTextProperty = DependencyProperty.Register("UninstallTex", typeof(string), typeof(ApplicationHolder), new PropertyMetadata(""));
        public static readonly DependencyProperty IsReadyProperty = DependencyProperty.Register("IsReady", typeof(bool), typeof(ApplicationHolder), new PropertyMetadata(true));
        public static readonly DependencyProperty DownloadCommandProperty = DependencyProperty.Register("DownloadCommand", typeof(ICommand), typeof(ApplicationHolder), new PropertyMetadata(null));
        public static readonly DependencyProperty AppInfoProperty = DependencyProperty.Register("AppInfo", typeof(AppInfo), typeof(ApplicationHolder), new PropertyMetadata(null, OnAppInfoChanged));

        private static void OnAppInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                ((ApplicationHolder) d).CheckForInstallation();
        }

        private void CheckForInstallation()
        {
            UninstallText=Shared.Helpers.RegistryHelper.GetUninstallCommand(AppInfo.Name);
            DownloadVisibility = String.IsNullOrEmpty(UninstallText) ? Visibility.Visible : Visibility.Collapsed;
            UninstallVisibility = String.IsNullOrEmpty(UninstallText) ? Visibility.Collapsed : Visibility.Visible;
        }

        public ApplicationHolder()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
