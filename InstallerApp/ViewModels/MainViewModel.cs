using InstallerApp.Views;
using Shared.Controls;
using Shared.Core;
using Shared.Models;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Shared.Helpers;
using Shared.Tools;

namespace InstallerApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isReady;
        private Pack _pack;

        public string CurrentVersion=> Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string XmlDataUrl { get; set; }

        public Pack Pack
        {
            get=>_pack;
            set
            {
                _pack = value;
                OnPropertyChanged();
            }
        }

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
                AppRunner.Run(((AppInfo) obj).GetUninstallStartInfo(), ReadPackFromSetting);
                Thread.Sleep(1000);
                ReadPackFromSetting();
                AlertBox.ShowMessage("Uninstalled", false);
                OnPropertyChanged((nameof(Pack)));
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
                ReadPackFromRemote();
            }
        }

        public void ReadPackFromRemote()
        {
            var url = SettingManager.GetDataPackUrl();
            if (string.IsNullOrEmpty(url) == false)
            {
                Pack = Downloader.DownloadXmlObject<Pack>(url);
                OnPropertyChanged(nameof(Pack.AppList));
                OnPropertyChanged(nameof(Pack));
            }
        }

        private void ExecuteInstallCommand(object obj)
        {
            var view = new DownloadView(obj as AppInfo);
            view.Closed += (s, e) => ReadPackFromSetting();
            view.Owner = Application.Current.MainWindow;
            view.Show();
        }

        private void ExecuteSyncCommand(object obj)
        {
            ReadPackFromSetting();
        }

        public void ReadPackFromSetting()
        {
            Pack = null;
            OnPropertyChanged(nameof(Pack));
            Thread.Sleep(1000);
            Pack = SettingManager.GetLocalDataPack();
            foreach (var appInfo in Pack.AppList)
            {
                RegistryTools.GetUninstallCommand(appInfo.Name, out var command, out var version);
                appInfo.UninstallCommand = command;
            }

            OnPropertyChanged(nameof(Pack));
        }

        public MainViewModel()
        {
            SettingCommand = new RelayCommand(ExecuteSettingCommand);
            SyncCommand = new RelayCommand(ExecuteSyncCommand);
            InstallCommand = new RelayCommand(ExecuteInstallCommand);
            UninstallCommand = new RelayCommand(ExecuteUninstallCommand);

            IsReady = false;
            ReadPackFromSetting();
        }
    }
}