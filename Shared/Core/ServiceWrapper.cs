using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Shared.API;
using Shared.Models;

namespace Shared.Core
{
    public class ServiceWrapper
    {
        private Models.Pack _pack;
        private API.FileDownloader _downloader;
        private System.Timers.Timer _timer;

        private void _downloader_FileDownloadCompleted(AppInfo appInfo)
        {
            InstallerTools.InstallDownloadedFileAsync(appInfo);
        }
        
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ServiceProcess();
        }

        public void Star()
        {
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = SettingManager.GetUpdateInterval();
            _timer.Start();

            _pack = SettingManager.ReadPackSetting();
            var appInfo = _pack.AppList[0];

            _downloader = new FileDownloader();
            _downloader.FileDownloadCompleted += _downloader_FileDownloadCompleted;
            _downloader.StartDownload(appInfo);
        }

        public void Stop()
        {
            _downloader.FileDownloadCompleted -= _downloader_FileDownloadCompleted;
            _downloader = null;

            _timer.Stop();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Dispose();
            _timer = null;
        }

        private void ServiceProcess()
        {
        }
    }
}
