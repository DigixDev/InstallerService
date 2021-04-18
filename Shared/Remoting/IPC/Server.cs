using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Shared.Controls;
using Shared.Core;
using Shared.Remoting.Interfaces;

namespace Shared.Remoting.IPC
{
    public class Server: IServer
    {
        private IpcChannel _serverChannel;
        private RemoteService _remoteService;
        private Action<string> _callback;

        public void Init(Action<string> callback)
        {
            try
            {
                _callback = callback;
                var clientProvider = new BinaryClientFormatterSinkProvider();
                var serverProvider = new BinaryServerFormatterSinkProvider();

                IDictionary prop = new Hashtable
                {
                    ["portName"] = GlobalData.REMOTE_SERVICE_CHANNEL,
                    ["authorizedGroup"] = "Everyone"
                };

                _serverChannel = new IpcChannel(prop, clientProvider, serverProvider);
                ChannelServices.RegisterChannel(_serverChannel, false);

                _remoteService = new RemoteService();
                _remoteService.Notify += OnNotifyReceived;

                RemotingServices.Marshal(_remoteService, GlobalData.REMOTE_SERVICE_NAME);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void CleanUp()
        {
            _remoteService.Notify -= OnNotifyReceived;
            ChannelServices.UnregisterChannel(_serverChannel);
            //RemotingServices.Unmarshal((ObjRef)_remoteService);

            //var channel =
            //    ChannelServices.RegisteredChannels.FirstOrDefault(x=>x.GlobalData.REMOTE_SERVICE_CHANNEL);
            //if (channel != null)
            //    ChannelServices.UnregisterChannel(channel);
        }

        private void OnNotifyReceived(string msg)
        {
            _callback.Invoke(msg);
        }

        public void Dispose()
        {
            CleanUp();
        }
    }
}
