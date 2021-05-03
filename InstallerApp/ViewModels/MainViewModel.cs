using InstallerApp.Views;
using Shared.Controls;
using Shared.Core;
using Shared.Models;
using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Shared.Helpers;
using Shared.Tools;

namespace InstallerApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isReady, _isUserAdmin;
        private Pack _pack;
        private Downloader _downloader;
        private MainWindow _parent;

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

        public bool IsUserAdmin
        {
            get => _isUserAdmin;
            set
            {
                _isUserAdmin = value;
                OnPropertyChanged();
            }
        }

        public ICommand SettingCommand { get; }
        public ICommand SyncCommand { get; }
        public ICommand InstallCommand { get; set; }
        public ICommand UninstallCommand { get; set; }

        private async void ExecuteUninstallCommand(object obj)
        {
            try
            {
                IsReady = false;

                await Task.Run(() =>
                {
                    AppRunner.Run(((AppInfo) obj).GetUninstallStartInfo(), ReadPackFromSetting);
                    Thread.Sleep(1000);
                });

                ReadPackFromSetting();
                OnPropertyChanged((nameof(Pack)));
                IsReady = true;
                AlertBox.ShowMessage("Uninstalled", false);

            }
            catch (Exception ex)
            {
                AlertBox.ShowMessage(ex.Message);
                IsReady = true;
            }
        }

        private void ExecuteSettingCommand(object obj)
        {
            IsReady = false;
            var view = new SettingView
            {
                Owner = Application.Current.MainWindow
            };

            if (view.ShowDialog() == true)
            {
                ReadPackFromSetting();
            }
            IsReady = true;
        }

        private void ExecuteInstallCommand(object obj)
        {
            IsReady = false;
            Start(obj as AppInfo);
            //var view = new DownloadView(obj as AppInfo);
            //view.Closed += (s, e) => ReadPackFromSetting();
            //view.Owner = Application.Current.MainWindow;
            //view.Closed += (s, e) => IsReady = true;
            //view.Show();
        }

        private void ExecuteSyncCommand(object obj)
        {
            IsReady = false;
            ReadPackFromSetting();
            IsReady = true;
        }

        public void Start(AppInfo appInfo)
        {
            IsReady = false;
            Notify(GlobalData.CMD_START);
            Notify(GlobalData.CMD_DOWNLOADING, appInfo.Name, "0");

            _downloader.StartDownload(appInfo);
        }

        private void StartInstalling(AppInfo appInfo)
        {
            Notify(GlobalData.CMD_INSTALLING, " ");
            InstallerTools.InstallDownloadedFileAsync(appInfo);
            Notify(GlobalData.CMD_STOP, " ");
            IsReady = true;
            AlertBox.ShowMessage("Installed", false);
        }

        private void Notify(params string[] msgs)
        {
            _parent.OnMessageReceived(String.Join(":", msgs));
        }

        public void ReadPackFromSetting()
        {
            Pack = SettingManager.ReadDataPack();
            OnPropertyChanged(nameof(Pack.AppList));
            OnPropertyChanged(nameof(Pack));
        }

        private async void CheckIfApplicatedIsUpdated()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(3000);
                var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var oldVersion = RegistryTools.GetUserValue(GlobalData.REGKEY_APP_VERSION, "").ToString();

                if (String.IsNullOrEmpty(oldVersion))
                    RegistryTools.SetUserValue(GlobalData.REGKEY_APP_VERSION, version);
                else if (version.Equals(oldVersion) == false)
                {
                    RegistryTools.SetUserValue(GlobalData.REGKEY_APP_VERSION, version);
                    AlertBox.ShowMessage($"Updated to version: {version}");
                }
            });
        }

        public MainViewModel(MainWindow parent)
        {
            SettingCommand = new RelayCommand(ExecuteSettingCommand, (e)=>IsReady);
            SyncCommand = new RelayCommand(ExecuteSyncCommand, (e) => IsReady);
            InstallCommand = new RelayCommand(ExecuteInstallCommand, (e) => IsReady);
            UninstallCommand = new RelayCommand(ExecuteUninstallCommand, (e) =>IsReady);
            
            IsReady = true;
            
            _parent = parent;
            _parent.Loaded += (s, e) => CheckIfApplicatedIsUpdated();

            _downloader=new Downloader();
            _downloader.DownloadCompleted += StartInstalling;
            _downloader.DownloadProgress += x => Notify(GlobalData.CMD_DOWNLOADING, " ", x.ToString()); ;

            ReadPackFromSetting();
        }
    }
}