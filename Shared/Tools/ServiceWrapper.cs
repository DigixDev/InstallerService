using System;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Shared.Core;
using Shared.Helpers;

namespace Shared.Tools
{
    public class ServiceWrapper
    {
        private Downloader _downloader;
        private ElapsedEventHandler _timerHandler;
        private System.Timers.Timer _timer;

        public DateTime LastUpdateTime { get; set; }

        private void FileDownloadCompleted(AppInfo appInfo)
        {
            InstallerTools.InstallDownloadedFileAsync(appInfo);
        }
        
        public void Star()
        {
            _timerHandler= (s, e) => CheckForUpdateOrTask();
            _timer = new Timer
            {
                Interval = 30_000 // SettingManager.GetUpdateInterval();
            };
            _timer.Elapsed += _timerHandler;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Elapsed -= _timerHandler;
            _timer.Dispose();
            _timer = null;
        }

        private async void CheckForUpdateOrTask()
        {
            if (TaskManager.Exists)
            {
                if (TaskManager.TaskReady)
                    TaskManager.DoCurrentTask();
                else
                    return;
            }

            var url = SettingManager.GetDataPackUrl();
            
            var remotePack =await _downloader.DownloadDataPack(url);
            var localPack = SettingManager.GetLocalDataPack();

            if (remotePack.InstallerVersion.Equals(localPack.InstallerVersion) == false)
                UpdateInstallerApp(remotePack);

            var tasks = FindTasks(localPack, remotePack);
            if (tasks.Length ==0)
                return;

            TaskManager.AddRange(tasks);

            UpdateSetting(remotePack);
        }

        private void UpdateInstallerApp(Pack remotePack)
        {
            AppRunner.RunUpdater(remotePack.InstallerDownloadUrl);
        }

        private void UpdateSetting(Pack remotePack)
        {
            foreach (var appInfo in remotePack.AppList)
                appInfo.UninstallCommand = RegistryTools.GetUninstallCommand(appInfo.Name);

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
            LastUpdateTime = DateTime.Now;
        }

    }
}
