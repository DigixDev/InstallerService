using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Core;

namespace Shared.Models
{
    public class SettingModel
    {
        public string JsonDataUrl { get; set; }
        public int Port { get; set; }
        public int Interval { get; set; }

        public SettingModel()
        {
            Port = GlobalData.DEFAULT_PORT;
            JsonDataUrl=String.Empty;
        }
    }
}
