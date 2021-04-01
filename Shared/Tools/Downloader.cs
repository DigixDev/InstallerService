using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
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

        public static T DownloadXmlObject<T>(string url)
        {
            var res = DownloadString(url);
            var obj = XmlTools.Deserialize<T>(res);
            return obj;
        }

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

        public Pack DownloadDataPack(string url)
        {
            return DownloadXmlObject<Models.Pack>(url);
        }

        public void StartDownload(AppInfo appInfo)
        {
            _appInfo = appInfo;
            _client.DownloadFileAsync(new Uri(appInfo.GetDownloadPath()), appInfo.LocalFileName);
        }

        public Downloader()
        {
            _client = new WebClient();
            _client.DownloadFileCompleted += (s, e) => DownloadCompleted?.Invoke(_appInfo);
            _client.DownloadProgressChanged += (s, e) => DownloadProgress?.Invoke(e.ProgressPercentage);
        }
    }
}