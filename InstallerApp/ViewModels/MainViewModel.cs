using InstallerApp.Views;
using Shared.Controls;
using Shared.Core;
using Shared.Models;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace InstallerApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isReady;

        public string CurrentVersion=> Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string XmlDataUrl { get; set; }

        public Pack Pack { get; private set; }

        public bool IsReady
        {
            get => _isReady;
            set
            {
                if (_isReady != value)
                {
                    _isReady = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SettingCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand InstallCommand { get; set; }
        public ICommand UninstallCommand { get; set; }

        private void ExecuteUninstallCommand(object obj)
        {
            try
            {
                AppRunner.RunWithCallback(((AppInfo) obj).GetUninstallStartInfo(), ReadSetting);
                ReadSetting();
                AlertBox.ShowMessage("Uninstalled", false);
            }
            catch (Exception ex)
            {
                AlertBox.ShowMessage(ex.Message);
            }
        }

        private void ExecuteSettingCommand(object obj)
        {
            var view = new SettingView
            {
                Owner = Application.Current.MainWindow
            };

            if (view.ShowDialog() == true)
            {
                ReadSetting();
            }
        }

        private void ExecuteInstallCommand(object obj)
        {
            var view = new DownloadView(obj as AppInfo);
            view.Closed += (s, e) => ReadSetting();
            view.Owner = Application.Current.MainWindow;
            view.Show();
            ReadSetting();
        }

        private void ExecuteSyncCommand(object obj)
        {
            ReadSetting();
        }

        private void ReadSetting()
        {
            var runUpdater = false;
            Pack = Shared.Core.SettingManager.ReadPackSetting(CurrentVersion, ref runUpdater);

            OnPropertyChanged(nameof(Pack));
        }

        public MainViewModel()
        {
            SettingCommand = new RelayCommand(ExecuteSettingCommand);
            SyncCommand = new RelayCommand(ExecuteSyncCommand);
            InstallCommand = new RelayCommand(ExecuteInstallCommand);
            UninstallCommand = new RelayCommand(ExecuteUninstallCommand);

            IsReady = false;
            ReadSetting();
        }
    }
}