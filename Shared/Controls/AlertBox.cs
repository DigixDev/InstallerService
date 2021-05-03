using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Timer = System.Timers.Timer;

namespace Shared.Controls
{
    public class AlertBox : WindowEx
    {
        private readonly Timer _timer;
        
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(AlertBox), new PropertyMetadata(""));


        private void CloseAlert()
        {
            Dispatcher.Invoke(() =>
            {
                var anim=new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(500)));
                anim.Completed += (s, e) =>
                {
                    this.Close();
                };
                this.BeginAnimation(OpacityProperty, anim);
            });
        }

       // public Window ActiveWindow => Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

       public static void ShowMessage(string message, bool isError = true, Window owner = null)
       {
           Application.Current.Dispatcher.Invoke(() =>
           {
               var alertOwner = owner ?? Application.Current.MainWindow;
               var brush = isError ? Brushes.DarkRed : Brushes.ForestGreen;

               var alert = new AlertBox
               {
                   Message = message,
                   Background = brush,
                   Owner = alertOwner,
                   Width = alertOwner.Width,
                   Foreground = Brushes.White
               };

               alert.ShowDialog();
           });
       }

       public AlertBox()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;

            _timer = new Timer(4000);
            _timer.Elapsed += (s, e) => CloseAlert();
            _timer.Start();

            SizeChanged += (s, e) =>
            {
                Left = Owner.Left + Owner.ActualWidth - ActualWidth;
                Top = Owner.Top - ActualHeight;
            };
        }

    }
}