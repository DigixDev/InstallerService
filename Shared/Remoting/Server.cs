﻿using System;
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

namespace Shared.Remoting
{
    public class Server
    {
        private readonly IpcChannel _serverChannel;
        private readonly RemoteService _remoteService;
        private static ProgressView _progressView;
        private Thread _thread;
        private Action<string> _callBack;

        public Server(Action<string> callBack=null)
        {
            _callBack = callBack;
            var clientProvider = new BinaryClientFormatterSinkProvider();
            var serverProvider = new BinaryServerFormatterSinkProvider();

            IDictionary prop=new Hashtable();
            prop["portName"] = GlobalData.REMOTE_SERVICE_CHANNEL;
            prop["authorizedGroup"] = "Everyone";

            _serverChannel = new IpcChannel(prop, clientProvider, serverProvider);
            ChannelServices.RegisterChannel(_serverChannel, false);

            _remoteService=new RemoteService();
            _remoteService.Notify += OnNotifyReceived;

            RemotingServices.Marshal(_remoteService, GlobalData.REMOTE_SERVICE_NAME);
        }

        private void OnNotifyReceived(string msg)
        {
            _callBack.Invoke(msg);
           
        }

        private void ShowProgressWindow()
        {
            _progressView = new ProgressView();
            _progressView.Show();
        }
    }
}
