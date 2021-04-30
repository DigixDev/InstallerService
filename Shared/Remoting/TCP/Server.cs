using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Shared.Remoting.Interfaces;
using Shared.Tools;

namespace Shared.Remoting.TCP
{
    public class Server: IServer
    {
        private TcpListener _listener;
        private Thread _thread;
        private bool _running;
        private Action<string> _callback;

        public void Init(Action<string> callback)
        {
            try
            {
                var port = SettingManager.Setting.Port;
                
                _callback = callback;
                _listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
                _thread = new Thread(new ThreadStart(StartThread)) {IsBackground = false};
                _thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _listener = null;
            }
        }

        private async void StartThread()
        {
            try
            {
                _running = true;
                _listener.Start();

                while (_running)
                {
                    var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    ProcessClient(client);
                }

                _listener.Stop();
            }
            catch (Exception)
            {
            }
        }

        private void ProcessClient(TcpClient client)
        {
            var buf = new byte[1024];
            using (var stream = client.GetStream())
            {
                var count = stream.Read(buf, 0, 1024);
                var msg = Encoding.ASCII.GetString(buf, 0, count);
                _callback.Invoke(msg);
            }
        }

        public void Dispose()
        {
            try
            {
                _running = false;
                if (_listener.Pending() == false)
                    _listener.Stop();

                if (_thread.Join(3000))
                    _thread.Abort();
            }
            catch (Exception) { }
        }
    }
}
