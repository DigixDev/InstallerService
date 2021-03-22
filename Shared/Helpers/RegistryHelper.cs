using System.Diagnostics;
using Microsoft.Win32;

namespace Shared.Helpers
{
    public static class RegistryHelper
    {
        public const string ApplicationName = "InstallerService";

        public static string GetApplicationPath(string fullPath)
        {
            var keyApp = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            return (string) keyApp.GetValue("ApplicationPath", fullPath);
        }

        public static void UpdateApplicationPath(string fullPath)
        {
            var keyApp = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            keyApp.SetValue("ApplicationPath", fullPath);

            var keyAutorun =
                Registry.CurrentUser.CreateSubKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run");
            keyAutorun.SetValue("InstallerService", fullPath);
        }

        public static string GetUninstallCommand(string appName)
        {
            string displayName;
            RegistryKey key;

            // search in: CurrentUser
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                Debug.WriteLine(displayName);
                if (string.IsNullOrEmpty(displayName) == false &&
                    displayName.ToLower().Contains(appName.ToLower()))
                    return (string) subkey.GetValue("UninstallString");
            }

            // search in: LocalMachine_32
            var localMachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            key = localMachine32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (string.IsNullOrEmpty(displayName) == false &&
                    displayName.ToLower().Contains(appName.ToLower()))
                    return (string) subkey.GetValue("UninstallString");
            }

            // search in: LocalMachine_64
            var localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            key = localMachine64.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (string.IsNullOrEmpty(displayName) == false &&
                    displayName.ToLower().Contains(appName.ToLower()))
                    return (string) subkey.GetValue("UninstallString");
            }

            // NOT FOUND
            return string.Empty;
        }

        public static void WriteValue(string keyName, string value)
        {
            var regKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            regKey.SetValue(keyName, value);
        }

        public static string ReadValue(string keyName)
        {
            var regKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            return (string) regKey.GetValue(keyName, string.Empty);
        }

        public static void WriteXamlDataUrl(string value)
        {
            var regKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            regKey.SetValue("XamlDataUrl", value);
        }

        public static string ReadXamlDataUrl()
        {
            var regKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            return (string) regKey.GetValue("XamlDataUrl", string.Empty);
        }
    }
}