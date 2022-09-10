using System;
using System.Globalization;
using System.Windows.Data;
using VectorGraphicEditor.Figures;

namespace VectorGraphicEditor.Converters
{
    internal class ToolStateToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DrawingMode)value;
        }
    }
}
