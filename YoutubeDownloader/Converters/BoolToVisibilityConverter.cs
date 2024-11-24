using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace YoutubeDownloader.Converters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool useHidden = parameter?.ToString() == "Hidden";
                return boolValue ? Visibility.Visible : (useHidden ? Visibility.Hidden : Visibility.Collapsed);
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class InvertedBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool useHidden = parameter?.ToString() == "Hidden";
                return boolValue ? (useHidden ? Visibility.Hidden : Visibility.Collapsed) : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
