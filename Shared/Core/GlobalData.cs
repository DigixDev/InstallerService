using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Shared.Helpers;

namespace Shared.Core
{
    public class GlobalData
    {
        public static string REMOTE_TARGET_ADDRESS => $"ipc://{REMOTE_SERVICE_CHANNEL}/{REMOTE_SERVICE_NAME}";

        public const string INSTALLER_NAME = "Installer Service";
        public const string FILE_INSTALLER="InstallerApp.exe";
        public const string FILE_SHARED="Shared.dll";
        public const string FILE_UPDATER= "Updater.exe";
        public const string FILE_PACK_JSON = "AppPack.json";
        public const string FILE_SETTING_JSON = "Setting.json";
        public const string FILE_REMOTE_PACK_ZIP = "AppPack.zip";
        public const string REGKEY_APP_FOLDER = "ApplicationDirectory";
        public const string PROCESS_NAME = "InstallerApp";
        public const string REMOTE_SERVICE_CHANNEL = "InstallerServiceChannel";
        public const string REMOTE_SERVICE_NAME = "ProgressService";
        public const string CMD_START = "START";
        public const string CMD_UNINSTALLING = "START";
        public const string CMD_STOP = "STOP";
        public const string CMD_DOWNLOADING = "DOWNLOADING";
        public const string CMD_INSTALLING = "INSTALLING";
        public const string CMD_UPDATING = "UPDATING";
        public const string PRODUCT_CODE = "{5D826AA1-6E24-4E9D-BB39-DE6975B2A690}";
        public const int DEFAULT_PORT = 48011;

        public static string GenerateDownloadedFileName(string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\InstallerService\\Download";
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            return Path.Combine(path, fileName.Replace(" ", "") + extension);
        }

        public static string GenerateLocalDataFileName(string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;
            
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\InstallerService";
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            return Path.Combine(path, fileName.Replace(" ", "") + extension);
        }

        public static string GenerateLocalDataFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\InstallerService";
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            return Path.Combine(path, fileName.Replace(" ", ""));
        }

        public static string GenerateInstalledFileName(string fileName)
        {
            var path = RegistryTools.GetApplicationPath();
            return Path.Combine(path, fileName);

        }

        //public static ProcessStartInfo ExtractUninstallScript(string script)
        //{
        //    var pattern = @"(?<exec>^([a-zA-Z]:\\)?[^\.]+\.(exe|msi))";

        //    var mc = Regex.Match(script.Trim(), pattern);
        //    if (mc.Success)
        //    {
        //        var exec = mc.Groups["exec"].Value;
        //        var param = script.Replace(exec, "");
        //        return new ProcessStartInfo(exec, param)
        //        {
        //            UseShellExecute = true
        //        };
        //    }

        //    return null;
        //}

        public static ProcessStartInfo ExtractUninstallScript(string script)
        {
            var pattern = @"(?<exec>^([a-zA-Z]:\\)?[^\.]+\.(exe|msi))";

            var mc = Regex.Match(script.Trim(), pattern);
            if (mc.Success)
            {
                var exec = mc.Groups["exec"].Value;
                var param = script.Replace(exec, "");
                return new ProcessStartInfo(exec, param)
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    ErrorDialog = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
            }

            return null;
        }

    }
}