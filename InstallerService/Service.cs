using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InstallerService
{
    public partial class Service : ServiceBase
    {

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }

        protected override void OnStop()
        {
        }
    }
}
