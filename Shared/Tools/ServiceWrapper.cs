using System;
using Shared.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Serilog;
using Shared.Core;
using Shared.Helpers;
using Timer = System.Timers.Timer;

namespace Shared.Tools
{
    public class ServiceWrapper
    {
        private Thread _thread;
        private AutoResetEvent _stopEvent;
        private DateTime? _lastCheckTime;
        private double _updateInterval;

        public void Start()
        {
            _stopEvent=new AutoResetEvent(false);
            _thread=new Thread(new ThreadStart(DoWork));
            _thread.Start();
        }

        private void DoWork()
        {
            try
            {
                Log.Information("Thread started");
                while (true)
                {

                    CheckForUpdateOrTask();
                    if (_stopEvent.WaitOne(20_000))
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
            _stopEvent.Set();
            _thread.Join();
        }

        private async void CheckForUpdateOrTask()
        {
            Log.Information("Check for update");
            try
            {
                if (TaskManager.Exists)
                {
                    if (TaskManager.TaskReady)
                        TaskManager.DoCurrentTask();
                    return;
                }

                if (_lastCheckTime != null)
                {
                    if (DateTime.Now.Subtract(_lastCheckTime.Value).TotalMinutes < _updateInterval)
                        return;
                }

                _lastCheckTime = DateTime.Now;

                var url = SettingManager.GetDataPackUrl();
                if (string.IsNullOrEmpty(url))
                    return;

                Log.Information("Data URL: " + url);
                var localPack = SettingManager.GetLocalDataPack();
                var remotePack = await Downloader.DownloadXmlObjectAsync<Models.Pack>(url);

                //var ver = AppRunner.GetCurrentApplicationVersion();
                //if (remotePack.InstallerVersion.Equals(ver) == false)
                //{
                //    Log.Information("Running updater");
                //    AppRunner.RunUpdater(remotePack.InstallerDownloadUrl);
                //}

                var tasks = FindTasks(localPack, remotePack);
                if (tasks.Length == 0)
                    return;

                Log.Information($"Tasks: {tasks.Length}" );
                TaskManager.AddRange(tasks);

                UpdateSetting(remotePack);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private void UpdateSetting(Pack remotePack)
        {
            foreach (var appInfo in remotePack.AppList)
            {
                RegistryTools.GetUninstallCommand(appInfo.Name, out var command, out var version);
                appInfo.UninstallCommand = command;
            }

            SettingManager.SetLocalDataPack(remotePack);
        }

        private TaskModel[] FindTasks(Pack localPack, Pack remotePack)
        {
            var tasks = new List<TaskModel>();

            //var uninstall = localPack.AppList.Where(x => remotePack.AppList.Any(z => z.Name.Equals(x.Name)) == false);
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
