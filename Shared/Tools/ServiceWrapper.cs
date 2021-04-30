using System;
using Shared.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Serilog;
using Shared.Core;
using Shared.Helpers;
using Shared.Remoting;
using Timer = System.Timers.Timer;

namespace Shared.Tools
{
    public class ServiceWrapper
    {
        private Thread _thread;
        private AutoResetEvent _stopEvent;
        private DateTime? _lastCheckTime;
        private static bool _isUpdating, _isFirst;

        public void Start()
        {
            try
            {
                _isFirst = true;
                _stopEvent=new AutoResetEvent(false);
                SettingManager.ReadSetting();
                _thread=new Thread(new ThreadStart(DoWork));

                Log.Information("Start Service");
                _thread.Start();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        private void DoWork()
        {
            try
            {
                Log.Information("Thread started");
                while (true)
                {
                    CheckForUpdateOrTask();
                    if (_stopEvent.WaitOne(3_000))
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                Log.Information("Stop Service");
                _stopEvent.Set();
                _thread.Abort();
            }
            catch (Exception)
            {
            }
        }

        private void CheckForUpdateOrTask()
        {
            try
            {
                if (_isUpdating)
                {
                    Log.Information("Ingnored because of running updater");
                    return;
                }

                if (TaskManager.Exists)
                {
                    if (TaskManager.TaskReady)
                        TaskManager.DoCurrentTask();
                    Log.Information("Ingnored because of existing Task");
                    
                    return;
                }

                if (_lastCheckTime != null)
                {
                    if (DateTime.Now.Subtract(_lastCheckTime.Value).TotalMinutes < SettingManager.Setting.Interval && _isFirst==false)
                        return;
                }

                _isFirst = false;
                _lastCheckTime = DateTime.Now;

                var url = SettingManager.Setting.JsonDataUrl;
                if (string.IsNullOrEmpty(url))
                    return;

                var localPack = SettingManager.ReadDataPack();
                var remotePack = Downloader.DownloadDataAppPack(url);

                var ver = AppRunner.GetCurrentApplicationVersion();
                if (remotePack.InstallerVersion.Equals(ver) == false && String.IsNullOrEmpty(ver) == false)
                {
                    _isUpdating = true;
                    TaskManager.Notify(GlobalData.CMD_UPDATING);
                    AppRunner.KillProcess();
                    var zipPath = Downloader.DownloadUpdateZipFile(remotePack.InstallerDownloadUrl);
                    AppRunner.RunUpdater(zipPath);
                   return;
                }

                _isUpdating = false;

                var tasks = new List<TaskModel>(FindTasks(localPack, remotePack));
                if (tasks.Count == 0)
                    return;

                Log.Information($"Task founded: {tasks.Count}");
                UpdateSettingAndValidate(remotePack, tasks);

                TaskManager.AddRange(tasks);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private void UpdateSettingAndValidate(Pack remotePack, List<TaskModel> tasks)
        {
            foreach (var appInfo in remotePack.AppList)
            {
                RegistryTools.GetUninstallCommand(appInfo.Name, out var command, out var version);
                appInfo.UninstallCommand = command;
                var find = tasks.SingleOrDefault(x => x.AppInfo.Name.Equals(appInfo.Name) && x.AppInfo.Version.Equals(version));
                if (find != null)
                    tasks.Remove(find);
            }

            SettingManager.WriteDataPack(remotePack);
        }

        private TaskModel[] FindTasks(Pack localPack, Pack remotePack)
        {
            var tasks = new List<TaskModel>();

            var install = remotePack.AppList.Where(x => localPack.AppList.Any(z => z.Name.Equals(x.Name)) == false);
            var update = remotePack.AppList.Where(x =>
                localPack.AppList.Any(z => z.Name.Equals(x.Name) && z.Version != x.Version));

           foreach (var appInfo in install)
               tasks.Add(new TaskModel(){AppInfo = appInfo, TaskType =TaskType.Install});

           foreach (var appInfo in update)
               tasks.Add(new TaskModel() {AppInfo = appInfo, TaskType = TaskType.Update});

           return tasks.ToArray();
        }

        public ServiceWrapper()
        {
            _lastCheckTime = DateTime.Now;
        }
    }
}
