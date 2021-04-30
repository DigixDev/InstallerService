using System.Threading;
using System.Windows;
using Updater.Core;

namespace Updater
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] Args;
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            if(IsSingleInstance())
                Args = e.Args;
        }

        private bool IsSingleInstance()
        {
            var mutex = new Mutex(false, "InstallerServicxe.Updater", out var isNew);
            if (isNew == false)
                Current.Shutdown();

            return isNew;
        }
    }
}