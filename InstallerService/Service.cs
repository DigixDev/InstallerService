using System.ServiceProcess;
using Shared.Core;
using Shared.Tools;

namespace InstallerService
{
    public partial class Service : ServiceBase
    {
        private ServiceWrapper _serviceWrapper;

        public Service()
        {
            InitializeComponent();
        }

    #if DEBUG

        public void Start()
        {
            OnStart(null);
        }

    #endif
        protected override void OnStart(string[] args)
        {
            _serviceWrapper=new ServiceWrapper();
            _serviceWrapper.Star();
        }

        protected override void OnStop()
        {
            _serviceWrapper.Stop();
        }
    }
}
