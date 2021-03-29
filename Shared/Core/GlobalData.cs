namespace Shared.Core
{
    public class GlobalData
    {
        public const string REMOTE_PACK_XML_FILE_NAME = "AppPack.xml";
        public const string REMOTE_PACK_ZIP_FILE_NAME = "AppPack.zip";
        public const string INSTALLER_APP_NAME = "InstallerApp.exe";
        public const string UPDATER_APP_NAME = "Updater.exe";
        public const string UUID = "8c0db2fa-79ca-11eb-9439-0242ac130002";
        public const string REG_UPDATE_INTERVAL = "UpdateInterval";
        public const string REGKEY_XML_DATA_URL = "XamlDataUrl";
        public const string REGKEY_XML_DATA_PACK = "XamlDataPack";
        public const string REGKEY_APP_FOLDER = "ApplicationDirectory";
        public const string PROCESS_NAME = "InstallerApp";
        public const string REMOTE_SERVICE_CHANNEL = "InstallerServiceChannel";
        public const string REMOTE_SERVICE_NAME = "ProgressService";
        public static string REMOTE_TARGET_ADDRESS => $"ipc://{REMOTE_SERVICE_CHANNEL}/{REMOTE_SERVICE_NAME}";
        public const string CMD_START = "START";
        public const string CMD_STOP = "STOP";
        public const string CMD_PROGRESS = "PROGRESS";

    }
}