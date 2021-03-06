using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Shared.Core;
using Shared.Models;

namespace Shared.Helpers
{
    public static class RegistryTools
    {
        public static string GetApplicationPath()
        {
            return (string) GetMachineValue(GlobalData.REGKEY_APP_FOLDER, "");
        }
       
        public static bool GetUninstallCommand(string appName, out string uninstallCommand, out string version)
        {
            string displayName, displayVersion;
            RegistryKey key;

            uninstallCommand = version = "";

            // search in: CurrentUser
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                displayVersion = subkey.GetValue("DisplayVersion") as string;
                Debug.WriteLine(displayName);
                if (string.IsNullOrEmpty(displayName) == false &&
                    displayName.ToLower().Contains(appName.ToLower()))
                {
                    uninstallCommand=(string) subkey.GetValue("UninstallString");
                    version = displayVersion;
                    return true;
                }
            }

            // search in: LocalMachine_32
            var localMachine32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            key = localMachine32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                displayVersion= subkey.GetValue("DisplayVersion") as string;
                if (string.IsNullOrEmpty(displayName) == false &&
                    displayName.ToLower().Contains(appName.ToLower()))
                {
                    uninstallCommand = (string)subkey.GetValue("UninstallString");
                    version = displayVersion;
                    return true;
                }
            }

            // search in: LocalMachine_64
            var localMachine64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            key = localMachine64.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (var keyName in key.GetSubKeyNames())
            {
                var subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                displayVersion = subkey.GetValue("DisplayVersion") as string;

                if (string.IsNullOrEmpty(displayName) == false &&
                    displayName.ToLower().Contains(appName.ToLower()))
                {
                    uninstallCommand = (string)subkey.GetValue("UninstallString");
                    version = displayVersion;
                    return true;
                }
            }

            return false;
        }

        public static object GetUserValue(string key, object defaultValue)
        {
            var reg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            return reg.GetValue(key, defaultValue);
        }

        public static void SetUserValue(string key, object value)
        {
            var reg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\InstallerService");
            reg.SetValue(key, value);
            reg.Close();
        }

        public static void SetLocalMachineValue(string key, object value)
        {
            var reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\InstallerService");
            reg.SetValue(key, value);
            reg.Close();
        }

        public static object GetMachineValue(string key, object defaultValue)
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\InstallerService");
            return reg.GetValue(key, defaultValue);
        }

        public static void UpdateUninstallCommand(Pack pack)
        {
            var cmd = "";

            if (pack == null)
                return;

            foreach (var appInfo in pack.AppList)
            {
                if (GetUninstallCommand(appInfo.Name, out cmd, out _))
                    appInfo.UninstallCommand = cmd;
            }
        }
    }
}