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
        private System.Timers.Timer _timer;
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer=new Timer(5000);
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
