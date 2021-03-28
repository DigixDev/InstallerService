using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Core;

namespace Shared.Remoting
{
    public class Client
    {
        private RemoteService _remoteService;

        public void Notify(string msg) => _remoteService.RaiseNotification(msg);

        public Client()
        {
            _remoteService =
                (RemoteService) Activator.GetObject(typeof(RemoteService), GlobalData.REMOTE_TARGET_ADDRESS);
        }
    }
}
