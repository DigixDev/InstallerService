using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Updater.API;
using Updater.Core;

namespace Updater.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private FileDownloader _downloader;
        
        private int _percent;
        private string _title;

      

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

        public async void StartUpdating()
        {
            await Task.Run(async () =>
            {
                Title = "Downloading";
                await LogFileAsync("Stopping Service");
                StopService();
                
                await LogFileAsync("Killing Process");
                KillProcess();

                await LogFileAsync($"Start downloading: {App.Args[0]}" );
                _downloader = new FileDownloader();
                _downloader.DownloadProgress += x => Percent = x;
                _downloader.FileDownloadCompleted += FileDownloadCompleted;
                _downloader.StartDownload(App.Args[0]);
            });
        }

        private async void ContinueUpdating(string zipFilePath)
        {
            try
            {
                await Task.Run(async () =>
                {
                    Title = "Updating";
                    var dirApp = GetInstallDirectory();
                    await LogFileAsync($"Start extracting: {zipFilePath} - {dirApp}");
                    ExtractFilesAndReplace(zipFilePath, dirApp);
                    
                    await RunAsync(dirApp, GlobalData.FILE_INSTALLER);

                    await LogFileAsync("Running Service");
                    StartService();
                    Exit();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExtractFilesAndReplace(string zipFilePath, string dirInstall)
        {
            var buf = new byte[1024];
            var count = 0;
            var dirSrc = Path.GetDirectoryName(zipFilePath);

            var zip= ZipFile.OpenRead(zipFilePath);
            
            foreach (var entity in zip.Entries)
            {
                var target = Path.Combine(dirInstall, entity.FullName);
                FileAPI.DeleteFile(target);
                using (var reader = entity.Open())
                using (var writer = File.Open(target, FileMode.CreateNew))
                {
                    while (true)
                    {
                        count = reader.Read(buf, 0, 1024);
                        if (count == 0)
                            break;
                        writer.Write(buf, 0, count);
                    }
                }
            }
        }

        private void MakeEmptyDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.GetFiles(dir))
                    FileAPI.DeleteFile(file);
            }
            else
                Directory.CreateDirectory(dir);
        }

        private string GetInstallDirectory()
        {
            var reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\InstallerService");
            return (string) reg.GetValue("ApplicationDirectory", "");
        }

       //private bool DeleteFile(string filePath)
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        try
        //        {
        //            if (File.Exists(filePath))
        //                File.Delete(filePath);
        //            return true;
        //        }
        //        catch (Exception)
        //        {
        //        }

        //        Thread.Sleep(200);
        //    }

        //    return false;
        //}

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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void KillProcess(string processName = GlobalData.PROCESS_NAME)
        {
            try
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (process.ProcessName.Contains(processName))
                        process.Kill();
                }
            }
            catch (Exception ex)
            {
                await LogFileAsync(ex.Message);
            }
        }

        private async Task RunAsync(string dir, String fileName)
        {
            var file = Path.Combine(dir, fileName);
            await LogFileAsync($"Start Running: {dir} - {file}");

            if (File.Exists(file) == false)
            {
                await LogFileAsync($"Running Not Exists: {file}");
                return;
            }

            await LogFileAsync($"Running: {file}");
            var process = new Process { StartInfo = { FileName = file, Arguments = "" } };
            process.Start();
            return;
        }

        private async void Exit()
        {
            await Task.Delay(3_000);
            Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
        }

        #endregion

        private async Task LogFileAsync(string message)
        {
            using (var file = File.AppendText("d:\\logTest.txt"))
            {
                await file.WriteLineAsync(message + "\n");
            }
        }

        private void LogFile(string message)
        {
            using (var file = File.AppendText("d:\\logTest.txt"))
            {
                file.WriteLine(message + "\n");
            }
        }

        public MainViewModel()
        {
            if (App.Args.Length == 0)
                Application.Current.Shutdown();
        }
    }
}