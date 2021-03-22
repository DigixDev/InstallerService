using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Updater.Annotations;
using Updater.API;

namespace Updater.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string INSTALLER_FILE_NAME = "InstallerService.exe";
        private const string SHARED_FILE_NAME = "Shared.dll";
        private FileDownloader _downloader;
        private int _percent;

        public MainViewModel()
        {
            if (App.Args.Length == 0)
                Application.Current.Shutdown();

            else if (CheckForSetup() == false)
                ChackForInstallerUpdate();
        }

        private string CurrentApplicationPath => Assembly.GetExecutingAssembly().Location;
        private string CurrentApplicationDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


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
                foreach (var process in processes)
                    process.Kill();
        }

        private void ChackForInstallerUpdate()
        {
            KillProcess();
            if (App.Args.Length > 0)
            {
                _downloader = new FileDownloader();
                _downloader.DownloadProgress += x => Percent = x;
                _downloader.FileDownloadCompleted += _downloader_FileDownloadCompleted;
                _downloader.StartDownload(App.Args[0]);
            }
        }

        private bool CheckForSetup()
        {
            if (App.Args.Contains("setup"))
            {
                StartInstaller();
                return true;
            }

            return false;
        }

        private void StartInstaller()
        {
            try
            {
                if (Directory.Exists(CurrentApplicationDir))
                {
                    var exec = Path.Combine(CurrentApplicationDir, "InstallerService.exe");

                    var process = new Process();
                    process.StartInfo.FileName = exec;
                    process.StartInfo.Arguments = "nocheck";
                    process.Start();
                }
            }
            finally
            {
                Application.Current.Shutdown();
            }
        }

        private void _downloader_FileDownloadCompleted(string fileName)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CleanUp(params string[] files)
        {
            foreach (var file in files)
                if (File.Exists(file))
                    File.Delete(file);
        }

        private void RunInstaller(string installerFile)
        {
            var process = new Process();
            process.StartInfo.FileName = installerFile;
            process.StartInfo.Arguments = "nocheck";
            process.Start();
            Application.Current.Shutdown();
        }
    }
}