using System.Diagnostics;
using System.ServiceProcess;

namespace Updater
{
    public static class Tools
    {
        public const string WINDOWS_SERVICE_NAME = "InstallerWindowsService";
        public const double SERVICE_TIMEOUT = 5000;
        public const string PROCESS_NAME = "InstallerApp";

        public static void StopService()
        {
            try
            {
                var service = new ServiceController(WINDOWS_SERVICE_NAME);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped,
                        System.TimeSpan.FromMilliseconds(SERVICE_TIMEOUT));
                }
            }
            catch (System.Exception ex)
            {
            }
        }

        public static void StartService()
        {
            try
            {
                var service = new ServiceController(WINDOWS_SERVICE_NAME);

                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running,
                        System.TimeSpan.FromMilliseconds(SERVICE_TIMEOUT));
                }
            }
            catch (System.Exception ex)
            {
            }
        }

        public static void KillProcess(string processName = PROCESS_NAME)
        {
            try
            {
                var processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (process.ProcessName.Contains(processName))
                        process.Kill();
                }
            }
            catch (System.Exception ex)
            {
            }
        }

    }
}