using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Core;
using Shared.Remoting.Interfaces;

namespace Shared.Remoting.IPC
{
    public class Client: IClient
    {
        private RemoteService _remoteService;

        public RemoteService RemoteService
        {
            get
            {
                if(_remoteService==null)
                    _remoteService = (RemoteService) Activator.GetObject(typeof(RemoteService), GlobalData.REMOTE_TARGET_ADDRESS);
                return _remoteService;
            }
        }

        public void Notify(params string[] msgs)
        {
            try
            {
                if (RemoteService == null)
                    return;
                RemoteService.RaiseNotification(String.Join(":", msgs));
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _remoteService = null;
            }
        }
    }
}
