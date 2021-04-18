using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Shared.Core;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Tools
{
    public static class AppRunner
    {
        private static Downloader _downloader;
        public static string CurrentApplicationDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static bool Run(string filePath, string args = "", bool abs=true)
        {
            try
            {
                if (File.Exists(filePath) == false && abs)
                    return true;

                var process = new Process {StartInfo = {FileName = filePath, Arguments = args}};
                process.Start();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool RunWait(string filePath, string args = "", bool abs = true)
        {
            try
            {
                if (File.Exists(filePath) == false && abs)
                    return true;

                var process = new Process { StartInfo = { FileName = filePath, Arguments = args } };
                process.Exited += (s, e) => { };
                process.Start();
                process.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool Run(ProcessStartInfo startInfo, Action callBack = null)
        {
            try
            {
                if (callBack == null)
                    callBack = new Action(() => { });

                var process = new Process
                    {EnableRaisingEvents = true, StartInfo = startInfo};
                process.Exited += (s, e) => callBack();
                process.Start();
                process.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void KillProcess(string processName = GlobalData.PROCESS_NAME)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
                process.Kill();
        }

        public static void RunUpdater(string url)
        {
            Run(SettingManager.GetUpdaterFullPath(), url);
        }

        public static void DownloadAndUpdate(string downloadUrl)
        {
            try
            {
                var path = Path.Combine(Path.GetTempPath(), "InstallerService");
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                var batchFile = Path.Combine(path, "install.bat");
                var setupFile = Path.Combine(path, "setup.msi");

                if(File.Exists(batchFile))
                    File.Delete(batchFile);

                if (File.Exists(setupFile))
                    File.Delete(setupFile);

                using (var file=File.CreateText(batchFile))
                {
                    file.WriteLine($"start \"Uninstall MSI\" /wait \"msiexec.exe\" /x \"{GlobalData.PRODUCT_CODE}\" /q");
                    file.WriteLine($"start \"Install MSI\" /wait \"msiexec.exe\" /i \"Setup.msi\" /qn+");
                }

                Downloader.DownloadFile(downloadUrl, setupFile);
                Run(batchFile);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                File.AppendAllText("e:\\error.txt", ex.Message+"\n");
            }
        }

        public static string GetCurrentApplicationVersion()
        {
            var fullPath = SettingManager.GetInstallerFullPath();
            
            if (File.Exists(fullPath) == false)
                return String.Empty;

            var assembly = Assembly.LoadFrom(fullPath);
            return assembly.GetName().Version.ToString();
        }

        public static void UninstallInstaller()
        {
            var cmd = $"MsiExec.exe /x {GlobalData.PRODUCT_CODE}";
            var processStartInfo = GlobalData.ExtractUninstallScript(cmd);
            Run(processStartInfo);
        }
    }
}
