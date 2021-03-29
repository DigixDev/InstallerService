using System.ServiceProcess;
using Shared.Core;
using Shared.Tools;

namespace InstallerService
{
    public partial class Service : ServiceBase
    {
        private readonly ServiceWrapper _serviceWrapper;

        public Service()
        {
            InitializeComponent();
            _serviceWrapper = new ServiceWrapper();
        }

        protected override void OnStart(string[] args)
        {
            _serviceWrapper.Start();
        }

        protected override void OnStop()
        {
            _serviceWrapper.Stop();
        }
    }
}
