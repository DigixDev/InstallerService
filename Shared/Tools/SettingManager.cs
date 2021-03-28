using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Core;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Tools
{
    public static class SettingManager
    {
        public static string ApplicationDirectory { get; set; }

        public static Pack GetLocalDataPack()
        {
            var dataUrl = RegistryTools.GetValue<string>(GlobalData.REGKEY_XML_DATA_URL, null);
            if (string.IsNullOrEmpty(dataUrl) == false)
            {
                var pack = XmlTools.Deserialize<Pack>(new Uri(dataUrl));
                return pack;
            }
            else
            {
                return new Pack();
            }
        }

        public static void SetLocalDataPack(Pack pack)
        {
            var xml = XmlTools.SerializeString<Pack>(pack);
            RegistryTools.SetValue(GlobalData.REGKEY_XML_DATA_PACK, xml);
        }

        public static double GetUpdateInterval()
        {
            var value = RegistryTools.GetValue<object>(GlobalData.REG_UPDATE_INTERVAL, 0);
            return Convert.ToDouble(value);
        }

        public static void SetUpdateInterval(double value)
        {
            RegistryTools.SetValue<object>(GlobalData.REG_UPDATE_INTERVAL, value);
        }

        public static string GetDataPackUrl()=> RegistryTools.GetValue<string>(GlobalData.REGKEY_XML_DATA_URL, null);

        public static void SetDataPackUrl(string url) => RegistryTools.SetValue<string>(GlobalData.REGKEY_XML_DATA_URL, url);

        static SettingManager()
        {
            ApplicationDirectory = RegistryTools.GetValue<string>(GlobalData.REGKEY_APP_FOLDER, null);
        }
    }
}
