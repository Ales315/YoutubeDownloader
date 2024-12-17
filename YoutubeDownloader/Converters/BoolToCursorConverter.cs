using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace YoutubeDownloader.Converters
{
    class BoolToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Cursors.Arrow : Cursors.Hand;
            }
            return Cursors.Arrow;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
