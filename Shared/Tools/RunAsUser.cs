using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Microsoft.Win32.SafeHandles;
using Serilog;

namespace Shared.Tools
{
    //https://docs.microsoft.com/en-us/dotnet/api/system.security.principal.windowsimpersonationcontext?redirectedfrom=MSDN&view=netframework-4.8
    public class RunAsUser
    {
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_LOGON_INTERACTIVE = 2;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        public static void RunV2(string userName, string password, Action action)
        {
            var domain = Environment.MachineName;
         //   var username=WindowsIdentity.GetCurrent().Name;
            var context = new WrapperImpersonationContext(domain, userName, password);
            context.Enter();
            // Execute code under other uses context
            context.Leave();
        }

        public static void Run(string userName, string password, Action action)
        {
            var domainName = Environment.MachineName;
            // Call LogonUser to obtain a handle to an access token.   
            var returnValue = LogonUser(userName, domainName, password, LOGON32_LOGON_INTERACTIVE,
                LOGON32_PROVIDER_DEFAULT, out var safeAccessTokenHandle);

            if (false == returnValue)
            {
                var ret = Marshal.GetLastWin32Error();
                throw new Exception($"LogonUser failed with error code : {ret}");
            }

            Log.Information("Before impersonation: " + WindowsIdentity.GetCurrent().Name);

            WindowsIdentity.RunImpersonated(safeAccessTokenHandle, () =>
                {
                    Log.Information("During impersonation: " + WindowsIdentity.GetCurrent().Name);
                    action.Invoke();
                }
            );

            // Check the identity again.  
            Log.Information("After impersonation: " + WindowsIdentity.GetCurrent().Name);
        }
    }
}
