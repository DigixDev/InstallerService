using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Tools
{
    public class Downloader
    {
        public delegate void DownloadProgressDelegate(int percent);
        public delegate void FileDownloadCompletedDelegate(AppInfo appInfo);

        public event DownloadProgressDelegate DownloadProgress;
        public event FileDownloadCompletedDelegate FileDownloadCompleted;        
        
        private readonly WebClient _client;
        private AppInfo _appInfo;

        public Downloader()
        {
            _client = new WebClient();
            _client.DownloadFileCompleted += (s, e) => FileDownloadCompleted?.Invoke(_appInfo);
            _client.DownloadProgressChanged += (s, e) => DownloadProgress?.Invoke(e.ProgressPercentage);
        }

        //private string DataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        //    "InstallerServiceData");

        private static async Task<T> DownloadXmlObjectAsync<T>(string url)
        {
            using (var client = new HttpClient())
            {
                using (var stream = await client.GetStreamAsync(url))
                {
                    var res = XmlTools.Deserialize<T>(stream);
                    return res;
                }
            }
        }

        public async Task<Pack> DownloadDataPack(string url)
        {
            return await DownloadXmlObjectAsync<Models.Pack>(url);
        }

        public void StartDownload(AppInfo appInfo)
        {
            _appInfo = appInfo;
            _client.DownloadFileAsync(new Uri(appInfo.GetDownloadPath()), appInfo.LocalFileName);
        }
    }
}