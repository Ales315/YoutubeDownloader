using System.Globalization;
using System.Windows.Data;

namespace YoutubeDownloader.Converters
{
    class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var lowOpacity = float.Parse(parameter.ToString()!)/10;
                return boolValue ? 1.0 : lowOpacity;
            }
            return 1;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
