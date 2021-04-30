using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Updater.Helpers;

namespace Updater.API
{
    public class FileDownloader
    {
        public delegate void DownloadProgressDelegate(int percent);

        public delegate void FileDownloadCompletedDelegate(string filePath);

        private readonly WebClient _client;
        private string _filePath;

        public FileDownloader()
        {
            _client = new WebClient();
            _client.DownloadFileCompleted += (s, e) => FileDownloadCompleted?.Invoke(_filePath);
            _client.DownloadProgressChanged += (s, e) => DownloadProgress?.Invoke(e.ProgressPercentage);
        }


        private string DataFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "InstallerServiceData");

        public event DownloadProgressDelegate DownloadProgress;
        public event FileDownloadCompletedDelegate FileDownloadCompleted;

        public static async Task<T> DownloadXmlObject<T>(string url)
        {
            using (var client = new HttpClient())
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
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\InstallerService";
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            _filePath = Path.Combine(path, fileName);

            return _filePath;
        }

        public string StartDownload(string fileUrl)
        {
            var target = MakeTargetFileName("AppPack.zip");
            _client.DownloadFileAsync(new Uri(fileUrl), target);

            return target;
        }
    }
}