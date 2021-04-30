using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Shared.Core;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Tools
{
    public static class SettingManager
    {
        private static SettingModel _setting;

        public static SettingModel Setting
        {
            get
            {
                if (_setting == null)
                    _setting = ReadSetting();
                return _setting;
            }
            set => _setting = value;
        }

        public static string GetInstallerFullPath()
        {
            return Path.Combine(RegistryTools.GetApplicationPath(), GlobalData.FILE_INSTALLER);
        }

        public static void WriteDataPack(Pack pack)
        {
            try
            {
                var fileName = GlobalData.GenerateLocalDataFileName(GlobalData.FILE_PACK_JSON);
                JsonTools.SerializeFile<Pack>(fileName, pack);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public static void WriteSetting(SettingModel setting)
        {
            try
            {
                var fileName = GlobalData.GenerateLocalDataFileName(GlobalData.FILE_SETTING_JSON);
                Setting = setting;
                JsonTools.SerializeFile(fileName, setting);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public static SettingModel ReadSetting()
        {
            try
            {
                var fileName = GlobalData.GenerateInstalledFileName(GlobalData.FILE_SETTING_JSON);

                if (File.Exists(fileName))
                {
                    using (var file = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    using (var reader = new StreamReader(file))
                    {
                        Setting = JsonTools.Deserialize<SettingModel>(reader.ReadToEnd());
                    }
                }
                else
                    Setting= new SettingModel();

                return Setting;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public static Pack ReadDataPack()
        {
            try
            {
                var fileName = GlobalData.GenerateLocalDataFileName(GlobalData.FILE_PACK_JSON);
                if (File.Exists(fileName) == false)
                    return new Pack();
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(file))
                {
                    var pack = JsonTools.Deserialize<Pack>(reader.ReadToEnd());
                    RegistryTools.UpdateUninstallCommand(pack);
                    return pack;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public static void UpdateLocalDataPack(string url)
        {
            try
            {
                var fileName = GlobalData.GenerateLocalDataFileName(GlobalData.FILE_PACK_JSON);
                if(string.IsNullOrEmpty(url))
                    return;

                var json = Downloader.DownloadString(url);
                File.WriteAllText(fileName, json);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        public static void UpdateLocalPackXml(Pack pack)
        {
            try
            {
                if(pack==null)
                    return;

                var fileName = GlobalData.GenerateLocalDataFileName(GlobalData.FILE_PACK_JSON);
                RegistryTools.UpdateUninstallCommand(pack);
                JsonTools.SerializeFile(fileName, pack);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}
