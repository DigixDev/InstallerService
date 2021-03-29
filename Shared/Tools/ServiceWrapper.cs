using System;
using Shared.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Shared.Core;
using Shared.Helpers;
using Timer = System.Timers.Timer;

namespace Shared.Tools
{
    public class ServiceWrapper
    {
        private Thread _thread;
        private AutoResetEvent _stopEvent;
        private static int _count = 0;

        public DateTime LastUpdateTime { get; set; }

        public void Start()
        {
            _stopEvent=new AutoResetEvent(false);
            _thread=new Thread(new ThreadStart(DoWork));
            _thread.Start();
        }

        private void LogFile(string msg)
        {
            using (var writer = File.AppendText("d:\\TestMe.txt"))
            {
                writer.WriteLine(msg);
            }
        }

        private void DoWork()
        {
            try
            {
                while (true)
                {
                    CheckForUpdateOrTask();
                    if (_stopEvent.WaitOne(1000))
                        break;
                }
            }
            catch (Exception ex)
            {
                LogFile(ex.Message);
            }
        }

        public void Stop()
        {
            _stopEvent.Set();
            _thread.Join();
        }

        private void CheckForUpdateOrTask()
        {
            Remoting.Client.Notify($"Hello {_count++}");
            return;
            if (TaskManager.Exists)
            {
                if (TaskManager.TaskReady)
                    TaskManager.DoCurrentTask();
                else
                    return;
            }

            var url = SettingManager.GetDataPackUrl();
            
            //var remotePack =await _downloader.DownloadDataPack(url);
            //var localPack = SettingManager.GetLocalDataPack();

            //if (remotePack.InstallerVersion.Equals(localPack.InstallerVersion) == false)
            //    UpdateInstallerApp(remotePack);

            //var tasks = FindTasks(localPack, remotePack);
            //if (tasks.Length ==0)
            //    return;

            //TaskManager.AddRange(tasks);

            //UpdateSetting(remotePack);
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
            //_timer = new Timer(1000);
            //_timer.Elapsed += (s, e) => CheckForUpdateOrTask();
        }
    }
}
