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
using Shared.Core;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Tools
{
    public static class AppRunner
    {
        public static string CurrentApplicationDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static bool Run(string filePath, string args = "")
        {
            try
            {
                if (File.Exists(filePath) == false)
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

        public static string GetCurrentApplicationVersion()
        {
            var fullPath = SettingManager.GetInstallerFullPath();
            
            if (File.Exists(fullPath) == false)
                return String.Empty;

            var assembly = Assembly.LoadFrom(fullPath);
            return assembly.GetName().Version.ToString();
        }
    }
}
