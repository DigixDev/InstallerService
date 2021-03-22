﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using InstallerService.UI.Views;
using Shared.Controls;
using Shared.Core;
using Shared.Helpers;
using Shared.Models;

namespace InstallerService.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _currentVersion;
        private bool _isReady;

        public MainViewModel()
        {
            SettingCommand = new RelayCommand(ExecuteSettingCommand);
            SyncCommand = new RelayCommand(ExecuteSyncCommand);
            InstallCommand = new RelayCommand(ExecuteInstallCommand);
            UninstallCommand = new RelayCommand(ExecuteUninstallCommand);

            CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            IsReady = false;
            ReadSetting();
        }

        public string CurrentVersion
        {
            get => _currentVersion;
            set
            {
                if (_currentVersion != value)
                {
                    _currentVersion = value;
                    OnPropertyChanged();
                }
            }
        }

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
            var view = new GetUrlView();
            view.DataUrl = RegistryHelper.ReadXamlDataUrl();
            view.Owner = Application.Current.MainWindow;
            if (view.ShowDialog() == true)
            {
                RegistryHelper.WriteXamlDataUrl(view.DataUrl);
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
            
            if (runUpdater)
                RunUpdater();

            OnPropertyChanged(nameof(Pack));
        }

        private void RunUpdater()
        {
            if (App.Args.Length > 0)
                return;

            var updaterFile = Path.Combine(AppRunner.CurrentApplicationDir, "updater.exe");
            Shared.Core.AppRunner.Run(updaterFile, Pack.InstallerDownloadUrl);
        }
    }
}