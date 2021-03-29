using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Tools
{
    public static class TaskManager
    {
        private static readonly List<Models.TaskModel> _taskList;

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

        public static void AddRange(TaskModel[] tasks) => _taskList.AddRange(tasks);

        public static void DoCurrentTask()
        {
            if (CurrentTask.TaskType == TaskType.Update)
                InstallerTools.Uninstall(CurrentTask.AppInfo);
        }

        static TaskManager()
        {
            _taskList=new List<TaskModel>();
        }


    }
}
