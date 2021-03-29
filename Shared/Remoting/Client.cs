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
    public static class Client
    {
        private static RemoteService _remoteService;

        public static RemoteService RemoteService
        {
            get
            {
                if(_remoteService==null)
                    _remoteService = (RemoteService) Activator.GetObject(typeof(RemoteService), GlobalData.REMOTE_TARGET_ADDRESS);
                return _remoteService;
            }
        }

        public static void Notify(string msg)
        {
            if (RemoteService == null)
                return;
            RemoteService.RaiseNotification(msg);
        }
    }
}
