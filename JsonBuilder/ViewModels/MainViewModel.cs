using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Shared.Core;
using Shared.Helpers;
using Shared.Models;
using Shared.Tools;
using JsonBuilder.Properties;

namespace JsonBuilder.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _downloadUrl;
        private AppInfo _selectedAppInfo;
        private string _version, _currentVersion;

        public MainViewModel()
        {
            SelectedAppInfo = new AppInfo();
            Pack = new Pack();
            Pack.AppList.Add(SelectedAppInfo);

            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version = Settings.Default.Version;
            DownloadUrl = Settings.Default.DownloadUrl;

            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            ExitCommand = new RelayCommand(ExecuteExitCommand);
            NewCommand = new RelayCommand(ExecuteNewCommand, CanExecuteNewCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand);
            OpenCommand = new RelayCommand(ExecuteOpenCommand);
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

        public AppInfo SelectedAppInfo
        {
            get => _selectedAppInfo;
            set
            {
                if (_selectedAppInfo != value)
                {
                    _selectedAppInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        public Pack Pack { get; set; }

        public string DownloadUrl
        {
            get => _downloadUrl;
            set
            {
                _downloadUrl = value;
                OnPropertyChanged();
            }
        }

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        private bool CanExecuteNewCommand(object arg)
        {
            return true;
        }

        private void ExecuteNewCommand(object obj)
        {
            SelectedAppInfo = new AppInfo();
            Pack.AppList.Add(SelectedAppInfo);
        }

        private void ExecuteDeleteCommand(object obj)
        {
            Pack.AppList.Remove(SelectedAppInfo);
            if (Pack.AppList.Count == 0)
                Pack.AppList.Add(new AppInfo());
        }

        private void ExecuteClearCommand(object obj)
        {
            Pack.AppList.Clear();
            Pack.InstallerDownloadUrl = string.Empty;
            Pack.Description = string.Empty;
            Pack.InstallerVersion = string.Empty;

            SelectedAppInfo = new AppInfo();
            Pack.AppList.Add(SelectedAppInfo);

            OnPropertyChanged(nameof(Pack));
        }

        private void ExecuteExitCommand(object obj)
        {
            Application.Current.Shutdown();
        }

        private void ExecuteSaveCommand(object obj)
        {
            try
            {
                var dlg = new SaveFileDialog
                {
                    FileName = GlobalData.FILE_PACK_JSON,
                    Filter = "JSON Files (*.json)|*.json"
                };

                if (dlg.ShowDialog() == true)
                {
                    if (File.Exists(dlg.FileName))
                        File.Delete(dlg.FileName);
                    JsonTools.SerializeFile(dlg.FileName, Pack);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExecuteOpenCommand(object obj)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    FileName = GlobalData.FILE_PACK_JSON,
                    Filter = "JSON Files (*.json)|*.json"
                };
                if (dlg.ShowDialog() == true)
                {
                    Pack = JsonTools.DeserializeFile<Pack>(dlg.FileName);
                    OnPropertyChanged(nameof(Pack));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}