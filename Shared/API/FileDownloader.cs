using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Shared.Helpers;
using Shared.Models;

namespace Shared.API
{
    public class FileDownloader
    {
        private readonly WebClient _client;
        private string _filePath;

        private string DataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "InstallerServiceData");

        public delegate void FileDownloadCompletedDelegate(string filePath);
        public delegate void DownloadProgressDelegate(int percent);

        public event DownloadProgressDelegate DownloadProgress;
        public event FileDownloadCompletedDelegate FileDownloadCompleted ;

        public static async Task<T> DownloadXmlObject<T>(string url)
        {
            using (var client=new HttpClient())
            {
                using (var stream = await client.GetStreamAsync(url))
                {
                    var res = XmlHelper.Deserialize<T>(stream);
                    return res;
                }
            }
        }

        private string MakeTargetFileName(string fileName)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\InstallerService";
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            _filePath= Path.Combine(path, fileName.Replace(" ", "") + ".exe");

            return _filePath;
        }

        public FileDownloader()
        {
           _client = new WebClient();
           _client.DownloadFileCompleted += (s, e) => FileDownloadCompleted?.Invoke(_filePath);
           _client.DownloadProgressChanged += (s, e) => DownloadProgress?.Invoke(e.ProgressPercentage);
        }

        public void StartDownload(AppInfo appInfo)
        {

            _client.DownloadFileAsync(new Uri(appInfo.GetDownloadPath()), MakeTargetFileName(appInfo.Name));
        }
    }
}
