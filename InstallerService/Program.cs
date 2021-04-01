﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Shared.Tools;
using Topshelf;
using  Topshelf.Logging;

namespace InstallerService
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var exitCode = HostFactory.Run(x =>
            {
                x.Service<ServiceWrapper>(s =>
                {
                    s.ConstructUsing(wrapper => new ServiceWrapper());
                    s.WhenStarted(wrapper => wrapper.Start());
                    s.WhenStopped(wrapper => wrapper.Stop());
                });
               
                x.UseSerilog();
                x.StartAutomaticallyDelayed();
                x.RunAsLocalSystem();
                x.SetServiceName("InstallerWindowsService");
                x.SetDisplayName("Installer Windows Service");
            });

            Environment.ExitCode = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
        }
    }
}
