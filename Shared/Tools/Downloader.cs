using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Shared.Core;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Tools
{
    public class Downloader
    {
        public delegate void DownloadProgressDelegate(int percent);
        public delegate void DownloadCompletedDelegate(AppInfo appInfo);

        public event DownloadProgressDelegate DownloadProgress;
        public event DownloadCompletedDelegate DownloadCompleted;        
        
        private readonly WebClient _client;
        private AppInfo _appInfo;

        public static string DownloadString(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var res = client.DownloadString(new Uri(url));
                    return res;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return String.Empty;
            }
        }

        public void StartDownload(AppInfo appInfo)
        {
            _appInfo = appInfo;
            Log.Information($"Start downloading: {appInfo.SetupFileName}");
            _client.DownloadFileAsync(new Uri(appInfo.GetDownloadPath()), appInfo.SetupFileName);
        }

        public Downloader()
        {
            _client = new WebClient();
            _client.DownloadFileCompleted += (s, e) => DownloadCompleted?.Invoke(_appInfo);
            _client.DownloadProgressChanged += (s, e) => DownloadProgress?.Invoke(e.ProgressPercentage);
        }

        public static void DownloadFile(string downloadUrl, string localFileName)
        {
            try
            {
                using (var client = new WebClient())
                {
                    Log.Information($"Download Remote: {downloadUrl}, Local:{localFileName}");
                    client.DownloadFile(downloadUrl, localFileName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public static Pack DownloadDataAppPack(string url)
        {
            if(string.IsNullOrEmpty(url))
                throw new ArgumentException("URL is empty");

            var json = DownloadString(url);
            if(string.IsNullOrEmpty(json))
                return new Pack();
            
            return JsonTools.Deserialize<Pack>(json);
        }

        public static string DownloadUpdateZipFile(string url)
        {
            var localPath = GlobalData.GenerateDownloadedFileName("AppPack", ".zip");
            using (var client = new WebClient())
            {
                client.DownloadFile(url, localPath);
            }
            Log.Information($"Downloading: {url} to:{localPath}");
            return localPath;
        }
    }
}