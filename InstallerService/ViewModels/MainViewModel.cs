using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InstallerService.Properties;
using InstallerService.Views;
using Microsoft.Win32;
using Shared.Core;
using Shared.Models;

namespace InstallerService.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        private bool _isReady;
        private string _currentVersion;

        private string CurrentApplicationPath => System.Reflection.Assembly.GetExecutingAssembly().Location;
        private string CurrentApplicationDir => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

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
        public ICommand DownloadCommand { get; set; }
        public ICommand UninstallCommand { get; set; }

        private void ExecuteUninstallCommand(object obj)
        {
            var process = new Process();
            process.StartInfo.FileName = obj.ToString();
            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;
            process.Start();
            process.WaitForExit();
            ReadSetting();
        }

        private void ExecuteSettingCommand(object obj)
        {
            var view = new Views.GetUrlView();
            view.DataUrl = Shared.Helpers.RegistryHelper.ReadXamlDataUrl();
            view.Owner = Application.Current.MainWindow;
            if (view.ShowDialog() == true)
            {
                Shared.Helpers.RegistryHelper.WriteXamlDataUrl(view.DataUrl);
                ReadSetting();
            }
        }

        private void ExecuteDownloadCommand(object obj)
        {
            var view=new DownloadView(obj as AppInfo);
            view.Closed += (s, e) => ReadSetting();
            view.Owner = Application.Current.MainWindow;
            view.Show();
            ReadSetting();
        }

        private void ExecuteSyncCommand(object obj)
        {
            ReadSetting();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            ReadSetting();
        }

        private void ReadSetting(bool delay=true)
        {
            if(delay)
                Thread.Sleep(1000);
            XmlDataUrl = Shared.Helpers.RegistryHelper.ReadXamlDataUrl();
            if (String.IsNullOrEmpty(XmlDataUrl) == false)
            {
                Pack = Shared.Helpers.XmlHelper.Deserialize<Pack>(new Uri(XmlDataUrl));
                if (Pack.InstallerVersion.Equals(CurrentVersion)==false)
                    RunUpdater();
            }
            else
                Pack = new Pack();
            OnPropertyChanged(nameof(Pack));
        }

        private void RunUpdater()
        {
            if(App.Args.Length>0)
                return;

            var updaterFile = Path.Combine(CurrentApplicationDir, "updater.exe");
            if(File.Exists(updaterFile)==false)
                return;
            
            var process=new Process();
            process.StartInfo.FileName = updaterFile;
            process.StartInfo.Arguments = Pack.InstallerDownloadUrl;
            process.Start();
        }

        public MainViewModel()
        {
            SettingCommand = new RelayCommand(ExecuteSettingCommand);
            SyncCommand = new RelayCommand(ExecuteSyncCommand);
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand);
            UninstallCommand = new RelayCommand(ExecuteUninstallCommand);

            CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            IsReady = false;
            ReadSetting();
        }
    }
}
