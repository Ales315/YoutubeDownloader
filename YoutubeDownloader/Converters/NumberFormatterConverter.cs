using System.Globalization;
using System.Windows.Data;

namespace YoutubeDownloader.Converters
{
    class NumberFormatterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long number)
            {
                if (number >= 1000000)
                    return $"{number / 1000000}Mln";
                else if (number >= 1000000000)
                    return $"{number / 1000000000}Mrd";
                return number.ToString("N0", CultureInfo.CurrentCulture);
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
