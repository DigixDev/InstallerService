using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace Shared.Controls
{
    public class AlertBox : WindowEx
    {
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(AlertBox), new PropertyMetadata(""));

        private readonly Timer _timer;

        public AlertBox()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;

            _timer = new Timer(3000);
            _timer.Elapsed += (s, e) => Close();

            SizeChanged += (s, e) =>
            {
                Left = Owner.Left + Owner.ActualWidth - ActualWidth;
                Top = Owner.Top - ActualHeight;
            };
        }

        public Window ActiveWindow => Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

        public string Message
        {
            get => (string) GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static void ShowMessage(string message, bool isError = true, Window owner = null)
        {
            var alertOwner = owner ?? Application.Current.MainWindow;
            var brush = isError ? Brushes.DarkRed : Brushes.ForestGreen;

            var alert = new AlertBox
            {
                Message = message,
                Background = brush,
                Owner = alertOwner,
                Width = alertOwner.Width
            };

            alert.ShowDialog();
        }
    }
}