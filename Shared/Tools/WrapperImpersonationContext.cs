using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tools
{
    public class WrapperImpersonationContext
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain,
            String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;

        private string _domain;
        private string _password;
        private string _username;
        private IntPtr _token;

        private WindowsImpersonationContext m_Context = null;

        protected bool IsInContext
        {
            get { return m_Context != null; }
        }

        public WrapperImpersonationContext(string domain, string username, string password)
        {
            _domain = domain;
            _username = username;
            _password = password;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Enter()
        {
            if (this.IsInContext) return;
            _token = new IntPtr(0);
            try
            {
                _token = IntPtr.Zero;
                bool logonSuccessfull = LogonUser(
                    _username,
                    _domain,
                    _password,
                    LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT,
                    ref _token);
                if (logonSuccessfull == false)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(error);
                }
                WindowsIdentity identity = new WindowsIdentity(_token);
                m_Context = identity.Impersonate();
            }
            catch (Exception exception)
            {
                // Catch exceptions here
            }
        }


        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public void Leave()
        {
            if (this.IsInContext == false) return;
            m_Context.Undo();

            if (_token != IntPtr.Zero) CloseHandle(_token);
            m_Context = null;
        }
    }
}
