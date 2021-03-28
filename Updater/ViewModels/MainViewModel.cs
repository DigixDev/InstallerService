using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Shared.Core;
using Shared.Tools;
using Updater.Annotations;
using Updater.API;

namespace Updater.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string INSTALLER_FILE_NAME = "InstallerApp.exe";
        private const string SHARED_FILE_NAME = "Shared.dll";
        private FileDownloader _downloader;
        private int _percent;


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

        private void UpdateInstallerApp()
        {
            AppRunner.KillProcess(GlobalData.PROCESS_NAME);

            _downloader = new FileDownloader();
            _downloader.DownloadProgress += x => Percent = x;
            _downloader.FileDownloadCompleted += FileDownloadCompleted;
            _downloader.StartDownload(App.Args[0]);
        }

        private void FileDownloadCompleted(string fileName)
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

                AppRunner.Run(installerFile);
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


        public MainViewModel()
        {
            if (App.Args.Length == 0)
                Application.Current.Shutdown();
            else
                UpdateInstallerApp();
        }
    }
}