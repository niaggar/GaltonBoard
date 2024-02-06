using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GaltonBoard.App.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Brushes.Green : Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var brush = (SolidColorBrush)value;
        return brush.Color == Colors.Green;
    }
}