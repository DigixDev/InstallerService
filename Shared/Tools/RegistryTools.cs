using System.Diagnostics;
using Microsoft.Win32;

namespace Shared.Helpers
{
    public static class RegistryTools
    {
        private static RegistryKey ApplicationRegistryKey =>
            Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");

        public static string GetApplicationPath(string fullPath)
        {
            return (string) ApplicationRegistryKey.GetValue("ApplicationPath", fullPath);
        }

        public static void UpdateApplicationPath(string fullPath)
        {
            ApplicationRegistryKey.SetValue("ApplicationPath", fullPath);

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

        public static T GetValue<T>(string key, T defaultValue)
        {
            return (T) ApplicationRegistryKey.GetValue(key, defaultValue);
        }

        public static void SetValue<T>(string key, T value)
        {
            ApplicationRegistryKey.SetValue(key, value);
        }
    }
}