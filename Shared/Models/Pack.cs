using System.Collections.ObjectModel;

namespace Shared.Models
{
    public class Pack
    {
        public Pack()
        {
            AppList = new ObservableCollection<AppInfo>();
        }

        public string InstallerVersion { get; set; }
        public string InstallerDownloadUrl { get; set; }
        public string Description { get; set; }
        public ObservableCollection<AppInfo> AppList { get; set; }
    }
}