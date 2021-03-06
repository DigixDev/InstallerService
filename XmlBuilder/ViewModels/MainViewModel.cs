using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Shared.Core;
using Shared.Models;
using MessageBox = System.Windows.MessageBox;

namespace XmlBuilder.ViewModels
{
    public class MainViewModel: ViewModelBase
    {
        private string _version, _currentVersion;
        private string _downloadUrl;
        private AppInfo _selectedAppInfo;

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
            get { return _downloadUrl; }
            set
            {
                _downloadUrl = value;
                OnPropertyChanged();
            }
        }
        public string Version
        {
            get { return _version; }
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
            SelectedAppInfo=new AppInfo();
            Pack.AppList.Add(SelectedAppInfo);
        }

        private void ExecuteDeleteCommand(object obj)
        {
            Pack.AppList.Remove(SelectedAppInfo);
            if(Pack.AppList.Count==0)
                Pack.AppList.Add(new AppInfo());
        }

        private void ExecuteClearCommand(object obj)
        {
            Pack.AppList.Clear();
            Pack.InstallerDownloadUrl = String.Empty;
            Pack.Description = String.Empty;
            Pack.InstallerVersion = String.Empty;
            Pack.ListVersion = String.Empty;
            
            SelectedAppInfo = new AppInfo();
            Pack.AppList.Add(SelectedAppInfo);

            OnPropertyChanged(nameof(Pack));
        } 

        private void ExecuteExitCommand(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ExecuteSaveCommand(object obj)
        {
            try
            {
                var dlg = new SaveFileDialog();
                dlg.FileName = "AppPack.xml";//GlobalData.APP_PACK_XML_FILE_NAME;
                dlg.Filter = "XML Files (*.xml)|*.xml";

                if (dlg.ShowDialog() == true)
                {
                    if(File.Exists(dlg.FileName))
                        File.Delete(dlg.FileName);
                    Shared.Helpers.XmlHelper.Serialize(dlg.FileName, Pack);
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
                var dlg = new OpenFileDialog();
                dlg.FileName = "AppPack.xml";//GlobalData.APP_PACK_FILE_NAME;
                dlg.Filter = "XML Files (*.xml)|*.xml";
                if (dlg.ShowDialog() == true)
                {
                    Pack= Shared.Helpers.XmlHelper.Deserialize<Shared.Models.Pack>(dlg.FileName);
                    OnPropertyChanged(nameof(Pack));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public MainViewModel()
        {
            SelectedAppInfo = new AppInfo();
            Pack=new Pack();
            Pack.AppList.Add(SelectedAppInfo);

            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version = Properties.Settings.Default.Version;
            DownloadUrl = Properties.Settings.Default.DownloadUrl;

            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            ExitCommand = new RelayCommand(ExecuteExitCommand);
            NewCommand = new RelayCommand(ExecuteNewCommand, CanExecuteNewCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand);
            OpenCommand = new RelayCommand(ExecuteOpenCommand);
        }
    }
}
