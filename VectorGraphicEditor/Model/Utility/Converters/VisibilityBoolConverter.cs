using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VectorGraphicEditor.Converters
{
    internal class VisibilityBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value == true)
            {
                return Visibility.Visible;
            }

            return Visibility.Hidden;
        }
    }
}
