using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Shared.Core;

namespace TestService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Shared.Core.ServiceWrapper _serviceWrapper;

        public MainWindow()
        {
            InitializeComponent();
            _serviceWrapper=new ServiceWrapper();
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((Button)sender).Name.Equals("StartButton"))
                    _serviceWrapper.Star();
                else
                    _serviceWrapper.Stop();
                Debugger.Break();
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }
    }
}
