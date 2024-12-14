using System.Globalization;
using System.Windows.Data;

namespace YoutubeDownloader.Converters
{
    class DoubleToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                doubleValue *= 100;
                if (doubleValue == 100)
                    return $"Done";
                else if (doubleValue == -100)
                    return "Failed";
                else if (doubleValue == -50)
                    return "Cancelled";
                return $"{doubleValue:0.0}%";
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
