using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace VectorGraphicEditor
{
    public class PictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            VisualBrush visualBrush = new VisualBrush((Visual)value)
            {
                Viewport = new Rect(0, 0, 1, 1)
            };

            return visualBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
