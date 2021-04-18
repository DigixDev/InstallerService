using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Shared.Core;

namespace Shared.Models
{
    public class AppInfo
    {
        private string _localFileName;

        public string Name { set; get; }
        public string DownloadPathX86 { get; set; }
        public string DownloadPathX64 { get; set; }
        public string IconPath { get; set; }
        public string Version { get; set; }
        public string InstallScript { get; set; }
        public string UninstallScript { get; set; }
        public string UninstallerParam { get; set; }

        [XmlIgnore] 
        public string UninstallCommand { get; set; }

        [XmlIgnore]
        public string LocalFileName
        {
            get
            {
                if (string.IsNullOrEmpty(_localFileName))
                    _localFileName = GlobalData.GenerateLocalFileName(Name);
                return _localFileName;
            }
        }

        #region methods

        public ProcessStartInfo GetInstallStartInfo()
        {
            if (string.IsNullOrEmpty(InstallScript))
                return new ProcessStartInfo(LocalFileName)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };

            var temp = InstallScript.Replace("{0}", LocalFileName);
            return ExtractScript(temp);
        }

        public ProcessStartInfo GetUninstallStartInfo()
        {
            var temp = "";

            if (string.IsNullOrEmpty(UninstallScript))
                temp = $"{UninstallCommand.Trim()} {UninstallerParam.Trim()}";
            else
                temp = UninstallScript.Replace("{0}", LocalFileName);

            return ExtractScript(temp);
        }



        private ProcessStartInfo ExtractScript(string script)
        {
            var pattern = @"(?<exec>^([a-zA-Z]:\\)?[^\.]+\.(exe|msi))";

            var mc = Regex.Match(script.Trim(), pattern);
            if (mc.Success)
            {
                var exec = mc.Groups["exec"].Value;
                var param = script.Replace(exec, "");
                return new ProcessStartInfo(exec, param)
                {
                    UseShellExecute = true
                };
            }

            return null;
        }

        public string GetDownloadPath()
        {
            if (Environment.Is64BitOperatingSystem)
                return String.IsNullOrEmpty(DownloadPathX64) ? DownloadPathX86 : DownloadPathX64;
            else
                return DownloadPathX86;
        }

        #endregion
    }
}