using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Shared.Core;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Tools
{
    public static class SettingManager
    {
        
        public static string ApplicationDirectory { get; set; }

        public static string GetInstallerFullPath()
        {
            return Path.Combine(RegistryTools.GetApplicationPath(), GlobalData.FILE_INSTALLER);
        }

        public static string GetUpdaterFullPath()
        {
            return Path.Combine(RegistryTools.GetApplicationPath(), GlobalData.FILE_UPDATER);
        }

        public static Pack GetLocalDataPack()
        {
            var data = (string)RegistryTools.GetValue(GlobalData.REGKEY_XML_DATA_PACK, null);
            if (string.IsNullOrEmpty(data) == false)
            {
                var pack = XmlTools.Deserialize<Pack>(data);
                return pack;
            }
            else
            {
                return new Pack();
            }
        }

        public static void SetLocalDataPack(Pack pack)
        {
            var xml = XmlTools.Serialize<Pack>(pack);
            RegistryTools.SetValue(GlobalData.REGKEY_XML_DATA_PACK, xml);
        }

        public static void SetLocalDataPackAndUrl(string url)
        {
            try
            {
                var xml = Downloader.DownloadString(url);
                RegistryTools.SetValue(GlobalData.REGKEY_XML_DATA_URL, url);
                RegistryTools.SetValue(GlobalData.REGKEY_XML_DATA_PACK, xml);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public static void SetPort(int port)
        {
            try
            {
                RegistryTools.SetValue(GlobalData.REGKEY_PORT, port);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public static int GetPort()
        {
            try
            {
                return Convert.ToInt32(RegistryTools.GetValue(GlobalData.REGKEY_PORT, GlobalData.DEFAULT_PORT));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return GlobalData.DEFAULT_PORT;
            }
        }

        public static double GetUpdateInterval()
        {
            return Convert.ToDouble(RegistryTools.GetValue(GlobalData.REG_UPDATE_INTERVAL, 0.0));
        }

        public static void SetUpdateInterval(double value)
        {
            RegistryTools.SetValue(GlobalData.REG_UPDATE_INTERVAL, value);
        }

        public static string GetDataPackUrl()=> (string) RegistryTools.GetValue(GlobalData.REGKEY_XML_DATA_URL, null);


        static SettingManager()
        {
            ApplicationDirectory = (string)RegistryTools.GetValue(GlobalData.REGKEY_APP_FOLDER, null);
        }

        //public static async void UpdateDataPackFromRemote(string url)
        //{
        //    try
        //    {
        //        var xml = await Downloader.DownloadString(url);
        //        SettingManager.SetLocalDataPack(xml);
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //}
    }
}
