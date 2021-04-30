using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tools
{
    public class UserIndentity
    {
        public static IntPtr LogonUser()=> WindowsIdentity.GetCurrent().Token;
        public static WindowsIdentity GetAnonymousUser() => WindowsIdentity.GetAnonymous();

        public static bool IsAuthenticated(IntPtr userToken)
        {
            var identity=new WindowsIdentity(userToken);
            return identity.IsAuthenticated;
        }

        public static bool IsAnonymous(IntPtr userToken)
        {
            var identity = new WindowsIdentity(userToken);
            return identity.IsAnonymous;
        }

        public static bool IsSystem(IntPtr userToken)
        {
            var identity = new WindowsIdentity(userToken);
            return identity.IsSystem;
        }

        public static string AuthenticationType(IntPtr userToken)
        {
            var identity = new WindowsIdentity(userToken);
            return identity.AuthenticationType;
        }

    }
}
