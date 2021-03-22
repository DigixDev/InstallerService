using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Core
{
    public static class SettingManager
    {
        public static Models.Pack ReadPackSetting(string currentVersion, ref bool runUpdater)
        {
            var dataUrl = RegistryHelper.ReadXamlDataUrl();
            if (string.IsNullOrEmpty(dataUrl) == false)
            {
                var pack = XmlHelper.Deserialize<Pack>(new Uri(dataUrl));
                runUpdater = (pack.InstallerVersion.Equals(currentVersion) == false);
                return pack;
            }
            else
            {
                return new Pack();
            }
        }
    }
}
