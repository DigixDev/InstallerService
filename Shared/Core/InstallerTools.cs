using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Controls;
using Shared.Models;

namespace Shared.Core
{
    public static class InstallerTools
    {
        public static void InstallDownloadedFileAsync(string fileName)
        {
            try
            {
                var appInfo=new AppInfo();
                    var process = new Process
                    {
                        StartInfo = appInfo.GetInstallStartInfo()
                    };
                    process.Start();
                    process.WaitForExit();
                    //IsFinished = true;
                    //Thread.Sleep(3500);
                    //_parent.Dispatcher.Invoke(() => _parent.Close());
            }
            catch (Exception ex)
            {
                //AlertBox.ShowMessage(ex.Message, true, _parent);
                //_parent.Close();
            }
        }
    }
}
