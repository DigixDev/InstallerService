using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Updater
{
    class Program
    {
        private static string _applicationFolder;

        private static string ApplicationFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationFolder))
                    _applicationFolder = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\InstallerService").GetValue("ApplicationDirectory").ToString();
                return _applicationFolder;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteFile(string lpFileName);

        static void Main(string[] args)
        {
            if(args.Length==0)
            {
                Console.WriteLine("\n\t===== This appliction is part of the service =====");
                Console.ReadKey();
            }

            Tools.KillProcess();

            Tools.StopService();

            ExtractFiles(args[0]);

            Tools.StartService();

            AppRunner.CreateProcessInConsoleSession(GenerateApplicationPath("InstallerApp.exe"), "update", true);
        }

        private static string GenerateApplicationPath(string fileName)
        {
            return Path.Combine(ApplicationFolder, fileName);
        }

        private static void Log(string msg)
        {
            File.WriteAllText("c:\\log\\error.txt", msg);
        }

        private static void ExtractFiles(string zipFilePath)
        {
            try
            {
                var zipFile = ZipFile.OpenRead(zipFilePath);

                foreach (var entry in zipFile.Entries)
                {
                    var fileName = GenerateApplicationPath(entry.FullName);

                    if (File.Exists(fileName))
                    {
                        if (DeleteFile(fileName))
                            Log($"File deleted: {fileName}");
                        else
                            Log($"File delete error: {fileName}");
                    }

                    using (var reader = entry.Open())
                    using (var writer = new FileStream(fileName, FileMode.CreateNew))
                    {
                        reader.CopyTo(writer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
        }
    }
}
