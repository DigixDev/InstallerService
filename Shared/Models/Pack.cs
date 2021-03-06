using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Pack
    {
        public string InstallerVersion { get; set; }
        public string ListVersion { get; set; }
        public string InstallerDownloadUrl { get; set; }
        public string Description { get; set; }
        public ObservableCollection<AppInfo> AppList { get; set; }

        public Pack()
        {
            AppList=new ObservableCollection<AppInfo>();
        }
    }
}
