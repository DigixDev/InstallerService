using System.Windows;
using System.Windows.Controls;

namespace Updater.Controls
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
                _closeButton.Click += (s, e) => Close();

            _minimizeButton = GetTemplateChild(MinimizeButtonName) as Button;
            if (_minimizeButton != null)
                _minimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
        }
    }
}