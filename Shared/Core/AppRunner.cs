using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Core
{
    public static class AppRunner
    {
        public static string CurrentApplicationDir => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static bool Run(string filePath, string args)
        {
            try
            {
                if (File.Exists(filePath) == false)
                    return true;

                var process = new Process();
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = args;
                process.Start();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool RunWithCallback(ProcessStartInfo processStartInfo, Action readSetting)
        {
            try
            {
                var process = new Process();
                process.EnableRaisingEvents = true;
                process.StartInfo.UseShellExecute = false;
                process.Exited += (s, e) => readSetting();
                process.Start();
                process.WaitForExit();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
