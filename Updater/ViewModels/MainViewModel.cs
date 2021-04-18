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

        [Flags]
        enum MoveFileFlags
        {
            MOVEFILE_REPLACE_EXISTING = 0x00000001,
            MOVEFILE_COPY_ALLOWED = 0x00000002,
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004,
            MOVEFILE_WRITE_THROUGH = 0x00000008,
            MOVEFILE_CREATE_HARDLINK = 0x00000010,
            MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x00000020
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeleteFile(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

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

                await LogFileAsync("Start downloading");
                _downloader = new FileDownloader();
                _downloader.DownloadProgress += x => Percent = x;
                _downloader.FileDownloadCompleted += FileDownloadCompleted;
                _downloader.StartDownload(App.Args[0]);
            });
        }

        private async void ContinueUpdating(string filePath)
        {
            try
            {
                await Task.Run(async () =>
                {
                    Title = "Updating";
                    var dirApp = GetInstallDirectory();
                    await LogFileAsync("Start extracting");
                    ExtractFilesAndReplace(filePath, dirApp);
                    
                    await LogFileAsync("Running");
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

        private void ExtractFilesAndReplace(string filePath, string dirInstall)
        {
            var dirSrc = Path.GetDirectoryName(filePath);
            var dirExtract = Path.Combine(dirSrc, "extract");
            
            MakeEmptyDirectory(dirExtract);
            
            ZipFile.ExtractToDirectory(filePath, dirExtract);
            
            var files = Directory.GetFiles(dirExtract);
            foreach (var file in files)
            {
                var target = Path.Combine(dirInstall, Path.GetFileName(file));
                MoveFileEx(file, target, MoveFileFlags.MOVEFILE_REPLACE_EXISTING);
            }
        }

        private void MakeEmptyDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.GetFiles(dir))
                    DeleteFile(file);
            }
            else
                Directory.CreateDirectory(dir);
        }

       private async void MoveFile(string dirSrc, string dirTarget, string fileName)
        {
            var src = Path.Combine(dirSrc, fileName);
            var target = Path.Combine(dirTarget, fileName);
            
            await LogFileAsync($"Deleting {target}");
            DeleteFile(target);
            
            await LogFileAsync($"Moving from: '{src}' to '{target}'");
            MoveFileEx(src, target, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
        }

        private string GetInstallDirectory()
        {
            var reg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            return (string) reg.GetValue("ApplicationDirectory", "");
        }

        private void DeleteFile(string dir, string fileName)
        {
            var filePath = Path.Combine(dir, fileName);
            DeleteFile(filePath);
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
            if (File.Exists(file) == false)
            {
                await LogFileAsync($"Running {file}");
                return;
            }

            await LogFileAsync($"Running {file}");
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

        public MainViewModel()
        {
            if (App.Args.Length == 0)
                Application.Current.Shutdown();
        }
    }
}