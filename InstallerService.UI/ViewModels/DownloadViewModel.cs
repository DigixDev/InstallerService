using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Shared.API;
using Shared.Controls;
using Shared.Core;
using Shared.Models;

namespace InstallerService.UI.ViewModels
{
    public class DownloadViewModel : ViewModelBase
    {
        private AppInfo _appInfo;
        private bool _isFinished;
        private readonly Window _parent;
        private int _percent;
        private string _status;

        public DownloadViewModel(Window parent, AppInfo appInfo)
        {
            _parent = parent;
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
            var downloader = new FileDownloader();

            Status = $"Downloading {AppInfo.Name} (version: {AppInfo.Version})";

            downloader.FileDownloadCompleted += StartInstalling;
            downloader.DownloadProgress += x => Percent = x;
            downloader.StartDownload(AppInfo);
        }

        private void StartInstalling(string filePath)
        {
            Status = $"Installing {AppInfo.Name} (version: {AppInfo.Version})";
            Shared.Core.InstallerTools.InstallDownloadedFileAsync(filePath);
            Status = "Done.";
            IsFinished = true;
        }
    }
}