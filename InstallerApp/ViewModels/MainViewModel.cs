using InstallerApp.Views;
using Shared.Controls;
using Shared.Core;
using Shared.Models;
using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Shared.Helpers;
using Shared.Tools;

namespace InstallerApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isReady, _isUserAdmin;
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

        private void ExecuteUninstallCommand(object obj)
        {
            try
            {
                IsReady = false;

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
            finally
            {
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
                ReadPackFromRemote();
            }
            IsReady = true;
        }

        private void ExecuteInstallCommand(object obj)
        {
            IsReady = false;
            var view = new DownloadView(obj as AppInfo);
            view.Closed += (s, e) => ReadPackFromSetting();
            view.Owner = Application.Current.MainWindow;
            view.Closed += (s, e) => IsReady = true;
            view.Show();
        }

        private void ExecuteSyncCommand(object obj)
        {
            IsReady = false;
            ReadPackFromSetting();
            IsReady = true;
        }

        public bool CheckIsUserAdministrator()
        {
            bool isAdmin;
            WindowsIdentity user = null;
            try
            {
                user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }
            return isAdmin;
        }

        public void ReadPackFromRemote()
        {
            var url = SettingManager.GetDataPackUrl();
            if (string.IsNullOrEmpty(url) == false)
            {
                Pack = Downloader.DownloadXmlObject<Pack>(url);
                RegistryTools.UpdateUninstallCommand(Pack);
                OnPropertyChanged(nameof(Pack.AppList));
                OnPropertyChanged(nameof(Pack));
            }
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
            SettingCommand = new RelayCommand(ExecuteSettingCommand, (e)=>IsReady);
            SyncCommand = new RelayCommand(ExecuteSyncCommand, (e) => IsReady);
            InstallCommand = new RelayCommand(ExecuteInstallCommand, (e) => IsReady);
            UninstallCommand = new RelayCommand(ExecuteUninstallCommand, (e) =>IsReady);

            IsReady = true;
            IsUserAdmin = CheckIsUserAdministrator();

            ReadPackFromSetting();
        }
    }
}