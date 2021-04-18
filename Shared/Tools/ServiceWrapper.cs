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
using Shared.Remoting;
using Timer = System.Timers.Timer;

namespace Shared.Tools
{
    public class ServiceWrapper
    {
        private Thread _thread;
        private AutoResetEvent _stopEvent;
        private DateTime? _lastCheckTime;
        private double _updateInterval;
        private static bool _isUpdating, _isFirst;


        public void Start()
        {
            _isFirst = true;
            _stopEvent=new AutoResetEvent(false);
            _updateInterval = SettingManager.GetUpdateInterval();
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
                _stopEvent.Set();
                _thread.Abort();
            }
            catch (Exception)
            {
            }
        }

        private void CheckForUpdateOrTask()
        {
            Log.Information("Check for update");
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
                    if (DateTime.Now.Subtract(_lastCheckTime.Value).TotalMinutes < Math.Max(_updateInterval, 10) && _isFirst==false)
                    {
                        Log.Information("Not update time");
                        return;
                    }
                }

                _isFirst = false;
                _lastCheckTime = DateTime.Now;

                var url = SettingManager.GetDataPackUrl();
                if (string.IsNullOrEmpty(url))
                    return;

                Log.Information("Data URL: " + url);
                var localPack = SettingManager.GetLocalDataPack();
                var remotePack = Downloader.DownloadXmlObject<Models.Pack>(url);

                var ver = AppRunner.GetCurrentApplicationVersion();
                if (remotePack.InstallerVersion.Equals(ver) == false && String.IsNullOrEmpty(ver) == false)
                {
                    _isUpdating = true;
                    Log.Information("Running updater");
                    File.AppendAllText("e:\\error.txt", "Running updater\n");

                    TaskManager.Notify(GlobalData.CMD_UPDATING);
                    AppRunner.RunUpdater(remotePack.InstallerDownloadUrl);
                }

                _isUpdating = false;

                var tasks = new List<TaskModel>(FindTasks(localPack, remotePack));
                if (tasks.Count == 0)
                    return;

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
