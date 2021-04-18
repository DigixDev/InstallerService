using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Remoting.Interfaces
{
    public interface IServer: IDisposable
    {
        void Init(Action<string> callback);
    }
}
