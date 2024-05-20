using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Horseshoe.NET
{
    public static class Auth
    {
        private const int logon32_provider_default = 0;
        private const int logon32_logon_interactive = 2;
        private const int logon32_logon_batch = 4;

        public static void ImpersonateUser(string userName, string password, string domain, bool useBatchLogon = false)
        {
            IntPtr ptr = new IntPtr(0);
            try
            {
                ptr = IntPtr.Zero;
                bool isLoggedOn;

                try
                {
                    // Logon user (returns primary access token - unmanaged code)
                    isLoggedOn = LogonUser
                    (
                        userName,
                        domain,
                        password,
                        useBatchLogon ? logon32_logon_batch : logon32_logon_interactive,
                        logon32_provider_default,
                        ref ptr
                    );
                }
                catch (Exception)
                {
                    //If Batch Interactive fails, we can retry non batch interactive as a backup.
                    if (useBatchLogon)
                    {
                        isLoggedOn = LogonUser
                        (
                            userName,
                            domain,
                            password,
                            logon32_logon_interactive,
                            logon32_provider_default,
                            ref ptr
                        );
                    }
                    else throw;
                }

                if (!isLoggedOn)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // Create new Windows identity from Access Token
                WindowsIdentity newId = new WindowsIdentity(ptr);
                if (newId == null)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                //Impersonate new identity
                WindowsImpersonationContext impersonatedUser = newId.Impersonate();
                if (impersonatedUser == null)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                CloseHandle(ptr);
            }
        }

        #region Core Windows Impersonation

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string userName, string domain, string password,
                                     int logonType, int provider,
                                     ref IntPtr tokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr ptr);

        #endregion
    }
}
