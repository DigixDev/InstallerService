using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Button _closeButton, _minimizeButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _closeButton = GetTemplateChild(CloseButtonName) as Button;
            if (_closeButton != null)
                _closeButton.Click += (s, e) => this.Close();

            _minimizeButton = GetTemplateChild(MinimizeButtonName) as Button;
            if (_minimizeButton != null)
                _minimizeButton.Click += (s, e) => this.WindowState=WindowState.Minimized;
        }

        public WindowEx()
        {
            Opacity = 0;

            this.Loaded += (s, e) =>
            {
                ShowAnimated();
            };
        }

        public void ShowAnimated()
        {
            var da = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)));
            da.From = 0;
            da.To = 1.0;
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            this.BeginAnimation(OpacityProperty, da);
        }
    }
}
