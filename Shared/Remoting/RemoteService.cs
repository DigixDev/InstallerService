using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Remoting
{
    public class RemoteService: MarshalByRefObject
    {
        public event Action<string> Notify;

        public void RaiseNotification(string msg)
        {
            Notify?.Invoke(msg);
        }
    }
}
