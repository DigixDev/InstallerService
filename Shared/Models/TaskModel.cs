using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class TaskModel
    {
        public AppInfo AppInfo { get; set; }
        public TaskType TaskType { get; set; }
        public bool IsWorking { get; set; }
    }
}
