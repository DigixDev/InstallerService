using Shared.Controls;
using System.Windows;

namespace Shared.Remoting
{
    /// <summary>
    /// Interaction logic for ProgressView.xaml
    /// </summary>
    public partial class ProgressView : WindowEx
    {
        public ProgressView()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => RelocateTheWindow();
        }

        private void RelocateTheWindow()
        {
            Left = SystemParameters.FullPrimaryScreenWidth - ActualWidth;
            Top = 10; // SystemParameters.FullPrimaryScreenHeight - ActualHeight;
        }

        public void NotificationReceived(string name, short percent = 0)
        {
            AppName.Text = name;
            InstallProgress.Value = percent;
        }
    }
}
