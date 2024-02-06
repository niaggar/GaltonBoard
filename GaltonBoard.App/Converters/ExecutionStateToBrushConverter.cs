using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GaltonBoard.Model.Enums;

namespace GaltonBoard.App.Converters;

public class ExecutionStateToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var state = (ExecutionStateEnum)value;
        return state switch
        {
            ExecutionStateEnum.NotStarted => Brushes.Orange,
            ExecutionStateEnum.Running => Brushes.Green,
            ExecutionStateEnum.Finished => Brushes.Blue,
            ExecutionStateEnum.Cancelled => Brushes.Red,
            _ => Brushes.Black,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var brush = (SolidColorBrush)value;
        return brush.Color == Colors.Green;
    }
}