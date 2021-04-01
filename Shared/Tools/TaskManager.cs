using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Serilog;
using Shared.Core;
using Shared.Models;

namespace Shared.Tools
{
    public static class TaskManager
    {
        private static readonly List<Models.TaskModel> _taskList;
        private static readonly Downloader _downloader;

        public static TaskModel CurrentTask => _taskList[0];

        public static bool Exists => _taskList.Count > 0;
        
        public static bool TaskReady => _taskList.Count > 0 && _taskList[0].IsWorking == false;

        public static void Clear() => _taskList.Clear();

        public static void Done()=> _taskList.RemoveAt(0);

        public static TaskModel GetTask()
        {
            var task = _taskList[0];
            task.IsWorking = true;
            
            return task;
        }

        public static void AddRange(IList<TaskModel> tasks) => _taskList.AddRange(tasks);

        public static void DoCurrentTask()
        {
            if (CurrentTask.IsWorking)
                return;

            CurrentTask.IsWorking = true;
            Remoting.Client.Notify(GlobalData.CMD_START);
            if (CurrentTask.TaskType == TaskType.Update)
            {
                Remoting.Client.Notify(GlobalData.CMD_UNINSTALLING, CurrentTask.AppInfo.Name);
                InstallerTools.Uninstall(CurrentTask.AppInfo);
            }

            Remoting.Client.Notify(GlobalData.CMD_DOWNLOADING, CurrentTask.AppInfo.Name, "0");
            _downloader.StartDownload(CurrentTask.AppInfo);
        }
        private static void TaskCompleted()
        {
            _taskList.RemoveAt(0);
        }

        static TaskManager()
        {
            _taskList=new List<TaskModel>();
            _downloader=new Downloader();
            Log.Information("Start downloading");
            _downloader.DownloadProgress += (x) =>
                Remoting.Client.Notify(GlobalData.CMD_DOWNLOADING,CurrentTask.AppInfo.Name,x.ToString());
            _downloader.DownloadCompleted += (appInfo) =>
            {
                Log.Information("Download completed");
                Remoting.Client.Notify(GlobalData.CMD_DOWNLOADING, appInfo.Name, "100");

                Log.Information("Installing");
                Remoting.Client.Notify(GlobalData.CMD_INSTALLING, appInfo.Name);
                InstallerTools.InstallDownloadedFileAsync(appInfo);

                Log.Information("Done");
                Remoting.Client.Notify(GlobalData.CMD_STOP);
                TaskCompleted();
            };
        }


    }
}
