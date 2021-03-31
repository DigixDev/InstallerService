using Shared.Core;
using Shared.Models;
using System.Windows;
using Shared.Tools;

namespace InstallerApp.ViewModels
{
    public class DownloadViewModel : ViewModelBase
    {
        private AppInfo _appInfo;
        private bool _isFinished;
        private int _percent;
        private string _status;

        public DownloadViewModel(Window parent, AppInfo appInfo)
        {
            AppInfo = appInfo;
            IsFinished = false;
        }

        private AppInfo AppInfo
        {
            get => _appInfo;
            set
            {
                _appInfo = value;
                OnPropertyChanged();
            }
        }

        public bool IsFinished
        {
            get => _isFinished;
            set
            {
                if (_isFinished != value)
                {
                    _isFinished = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Percent
        {
            get => _percent;
            set
            {
                if (_percent != value)
                {
                    _percent = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Start()
        {
            var downloader = new Downloader();

            Status = $"Downloading {AppInfo.Name} (version: {AppInfo.Version})";

            downloader.DownloadCompleted += StartInstalling;
            downloader.DownloadProgress += x => Percent = x;
            downloader.StartDownload(AppInfo);
        }

        private void StartInstalling(AppInfo appInfo)
        {
            Status = $"Installing {AppInfo.Name} (version: {AppInfo.Version})";
            InstallerTools.InstallDownloadedFileAsync(appInfo);
            Status = "Done.";
            IsFinished = true;
        }
    }
}