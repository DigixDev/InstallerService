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
            var dataUrl = RegistryHelper.GetValue<string>(RegistryHelper.XML_DATA_URL, null);
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

        public static Models.Pack ReadPackSetting()
        {
            var dataUrl = RegistryHelper.GetValue<string>(RegistryHelper.XML_DATA_URL, null);
            if (string.IsNullOrEmpty(dataUrl) == false)
            {
                var pack = XmlHelper.Deserialize<Pack>(new Uri(dataUrl));
                return pack;
            }
            else
            {
                return new Pack();
            }
        }

        public static void SetUpdateInterval(double value)
        {
            RegistryHelper.SetValue<object>(RegistryHelper.UPDATE_INTERVAL, value);
        }

        public static string GetDataPackUrl()=> RegistryHelper.GetValue<string>(RegistryHelper.XML_DATA_URL, null);

        public static void SetDataPackUrl(string url) => RegistryHelper.SetValue<string>(RegistryHelper.XML_DATA_URL, url);

        public static double GetUpdateInterval()
        {
            var value = RegistryHelper.GetValue<object>(RegistryHelper.UPDATE_INTERVAL, 0);
            return Convert.ToDouble(value);
        }
    }
}
