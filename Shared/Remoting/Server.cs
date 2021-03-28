using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Shared.Core;

namespace Shared.Remoting
{
    public class Server
    {
        private IpcChannel _serverChannel;
        private RemoteService _remoteService;

        public Server()
        {
            _serverChannel=new IpcChannel(GlobalData.REMOTE_SERVICE_CHANNEL);
            ChannelServices.RegisterChannel(_serverChannel, false);

            _remoteService=new RemoteService();
            _remoteService.Notify += OnNotifyReceived;

            RemotingServices.Marshal(_remoteService, GlobalData.REMOTE_SERVICE_NAME);
        }

        private void OnNotifyReceived(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
