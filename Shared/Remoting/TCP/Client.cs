using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Shared.Remoting.Interfaces;
using Shared.Tools;

namespace Shared.Remoting.TCP
{
    public class Client: IClient
    {
        public void Notify(params string[] msgs)
        {
            try
            {
                var port = SettingManager.Setting.Port;

                using (var client = new TcpClient())
                {
                    client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
                    using (var stream = client.GetStream())
                    {
                        var buf = Encoding.ASCII.GetBytes(String.Join(":", msgs));
                        stream.Write(buf, 0, buf.Length);
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
