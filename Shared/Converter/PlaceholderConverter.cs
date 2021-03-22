using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shared.Converter
{
    public class PlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string) value;
            if (string.IsNullOrEmpty(text))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}