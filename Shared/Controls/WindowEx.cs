using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Shared.Controls
{
    [TemplatePart(Name = CloseButtonName, Type = typeof(Button))]
    [TemplatePart(Name = MinimizeButtonName, Type = typeof(Button))]
    public class WindowEx : Window
    {
        private const string CloseButtonName = "PART_CloseButton";
        private const string MinimizeButtonName = "PART_MinimizeButton";
        protected Button _closeButton, _minimizeButton;

        public WindowEx()
        {
            Loaded += (s, e) =>
            {
                var da = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
                da.From = 0;
                da.To = 1.0;
                da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                da.Completed += (sender, args) => Start();
                BeginAnimation(OpacityProperty, da);
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _closeButton = GetTemplateChild(CloseButtonName) as Button;
            if (_closeButton != null)
                _closeButton.Click += (s, e) => Close();

            _minimizeButton = GetTemplateChild(MinimizeButtonName) as Button;
            if (_minimizeButton != null)
                _minimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
        }

        public void ShowAnimated()
        {
            var da = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
            da.From = 0;
            da.To = 1;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            BeginAnimation(OpacityProperty, da);
        }

        protected virtual void Start()
        {
        }
    }
}