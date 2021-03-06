using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;

namespace Shared.Models
{
    public class AppInfo
    {
        public string Name { get; set; }
        public string DownloadPathX86 { get; set; }
        public string DownloadPathX64 { get; set; }
        public string IconPath { get; set; }
        public string Version { get; set; }

        public bool IsEmpty()
        {
            if (String.IsNullOrEmpty(Name) ||
                String.IsNullOrEmpty(IconPath) ||
                String.IsNullOrEmpty(Version) ||
                String.IsNullOrEmpty(DownloadPathX64) ||
                String.IsNullOrEmpty(DownloadPathX86))
                return false;
            return true;
        }

        public string GetDownloadPath() => Environment.Is64BitOperatingSystem ? DownloadPathX64 : DownloadPathX86;
    }
}
