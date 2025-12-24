using System;
using System.Globalization;
using System.Windows.Data;

namespace TaskManager
{
    public class UnixToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long unix)
                return DateTimeOffset.FromUnixTimeSeconds(unix).ToLocalTime().ToString("dd.MM.yyyy");
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
