using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InstallerService.UI.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isInstalling = System.Convert.ToBoolean(parameter);
            if (string.IsNullOrEmpty((string) value))
                return isInstalling ? Visibility.Visible : Visibility.Collapsed;
            return isInstalling ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}