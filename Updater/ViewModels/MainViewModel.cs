using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Updater.API;
using Updater.Core;

namespace Updater.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private FileDownloader _downloader;
        
        private int _percent;
        private string _title;

        private string CurrentApplicationDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
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

        private void FileDownloadCompleted(string fileName)
        {
           ContinueUpdating(fileName);
        }

        #region MyRegion

        public async void StartUpdating()
        {
            await Task.Run(() =>
            {
                Title = "Closing application";
                KillProcess(GlobalData.PROCESS_NAME);

                Title = "Stopping services";
                StopService();

                Title = "Downloading";
                _downloader = new FileDownloader();
                _downloader.DownloadProgress += x => Percent = x;
                _downloader.FileDownloadCompleted += FileDownloadCompleted;
                _downloader.StartDownload(App.Args[0]);
            });
        }

        private async void ContinueUpdating(string fileName)
        {
            try
            {
                await Task.Run(() =>
                {
                    var appData = Path.GetDirectoryName(fileName);
                    var tempInstaller = Path.Combine(appData, GlobalData.FILE_INSTALLER);
                    var tempShared = Path.Combine(appData, GlobalData.FILE_SHARED);

                    var installerFile = Path.Combine(CurrentApplicationDir, GlobalData.FILE_INSTALLER);
                    var sharedFile = Path.Combine(CurrentApplicationDir, GlobalData.FILE_SHARED);

                    CleanUp(tempInstaller, tempShared, installerFile, sharedFile);

                    ZipFile.ExtractToDirectory(fileName, appData);

                    File.Copy(tempInstaller, installerFile);
                    File.Copy(tempShared, sharedFile);

                    Title = "Running application";
                    Run(installerFile);

                    Title = "Starting service";
                    StartService();

                    Title = "Done";
                    Exit();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region system files


        private void StopService()
        {
            try
            {
                var service = new ServiceController(GlobalData.WINDOWS_SERVICE_NAME);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped,
                        TimeSpan.FromMilliseconds(GlobalData.SERVICE_TIMEOUT));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StartService()
        {
            try
            {
                var service = new ServiceController(GlobalData.WINDOWS_SERVICE_NAME);

                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running,
                        TimeSpan.FromMilliseconds(GlobalData.SERVICE_TIMEOUT));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void KillProcess(string processName = GlobalData.PROCESS_NAME)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
                process.Kill();
        }

        private void Run(String fileName)
        {
            if (File.Exists(fileName) == false)
                return;

            var process = new Process {StartInfo = {FileName = fileName, Arguments = ""}};
            process.Start();
        }

        private void CleanUp(params string[] files)
        {
            foreach (var file in files)
                if (File.Exists(file))
                    File.Delete(file);
        }

        private async void Exit()
        {
            await Task.Delay(3_000);
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        #endregion

        public MainViewModel()
        {
            if (App.Args.Length == 0)
                Application.Current.Shutdown();
        }
    }
}