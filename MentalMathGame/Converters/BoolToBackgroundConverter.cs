using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MentalMathGame.Converters;

public class BoolToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isSelected = value is bool b && b;
        string key = isSelected ? "AccentBrush" : "CardBrush";
        return (SolidColorBrush)Application.Current.Resources[key];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
