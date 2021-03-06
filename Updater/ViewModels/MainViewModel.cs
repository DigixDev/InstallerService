using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Updater.Annotations;
using Updater.API;

namespace Updater.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private API.FileDownloader _downloader;
        private int _percent;

        private string CurrentApplicationPath => System.Reflection.Assembly.GetExecutingAssembly().Location;
        private string CurrentApplicationDir => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        private const string INSTALLER_FILE_NAME = "InstallerService.exe";
        private const string SHARED_FILE_NAME = "Shared.dll";

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void KillProcess(string name = "InstallerService")
        {
            var processes = Process.GetProcessesByName(name);
            if (processes != null)
            {
                foreach (var process in processes)
                {
                    process.Kill();
                }
            }
        }

        public MainViewModel()
        {
            KillProcess();
            _downloader=new FileDownloader();
            _downloader.DownloadProgress += (x => Percent = x);
            _downloader.FileDownloadCompleted += _downloader_FileDownloadCompleted;
            _downloader.StartDownload(App.Args[0]);
        }

        private void _downloader_FileDownloadCompleted(string fileName)
        {
            var appData = Path.GetDirectoryName(fileName);
            var tempInstaller = Path.Combine(appData, INSTALLER_FILE_NAME);
            var tempShared = Path.Combine(appData, SHARED_FILE_NAME);

            var installerFile = Path.Combine(CurrentApplicationDir, INSTALLER_FILE_NAME);
            var sharedFile = Path.Combine(CurrentApplicationDir, SHARED_FILE_NAME);

            CleanUp(tempInstaller, tempShared, installerFile, sharedFile);

            ZipFile.ExtractToDirectory(fileName, appData);

            File.Copy(tempInstaller, installerFile);
            File.Copy(tempShared, sharedFile);
            
            RunInstaller(installerFile);
        }

        private void CleanUp(params string[] files)
        {
            foreach (var file in files)
            {
                if(File.Exists(file))
                    File.Delete(file);
            }
        }

        private void RunInstaller(string installerFile)
        {
            var process=new Process();
            process.StartInfo.FileName = installerFile;
            process.StartInfo.Arguments = "nocheck";
            process.Start();
            Application.Current.Shutdown();
        }
    }
}
