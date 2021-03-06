using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using InstallerService.Views;
using Shared.Core;
using Shared.Models;

namespace InstallerService.ViewModels
{
    public class DownloadViewModel: ViewModelBase
    {
        private string _status;
        private AppInfo _appInfo;
        private bool _isFinished;
        private int _percent;
        private Window _parent;

        private Shared.Models.AppInfo AppInfo
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

        private void Start()
        {
            Status = $"Downloading {AppInfo.Name} (version: {AppInfo.Version})";
            StartDownloading();
        }

        private void StartDownloading()
        {
            var downloader=new Shared.API.FileDownloader();
            downloader.FileDownloadCompleted += StartInstalling;
            downloader.DownloadProgress += (x) => Percent = x;
            downloader.StartDownload(AppInfo);
        }

        private async void StartInstalling(string filePath)
        {
            Status = $"Installing {AppInfo.Name} (version: {AppInfo.Version})";
            await InstallDownloadedFileAsync(filePath);
            Status = "Done.";
            IsFinished = true;
        }

        private async Task InstallDownloadedFileAsync(string filePath)
        {
            await Task.Run(() =>
            {
                var process = new Process { StartInfo = { FileName = filePath } };
                process.Start();
                process.WaitForExit();
                IsFinished = true;
                Thread.Sleep(3500);
                _parent.Dispatcher.Invoke(() => _parent.Close());
            });
        }

        public DownloadViewModel(Window parent, AppInfo appInfo)
        {
            _parent = parent;

            AppInfo = appInfo;
            IsFinished = false;

            Start();
        }
    }
}
