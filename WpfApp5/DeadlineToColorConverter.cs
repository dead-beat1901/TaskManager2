using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager
{
    public class DeadlineToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is long))
                return Brushes.Transparent;

            DateTime deadline =
                DateTimeOffset.FromUnixTimeSeconds((long)value).LocalDateTime.Date;

            DateTime today = DateTime.Now.Date;

            if (deadline < today)
                return new SolidColorBrush(Color.FromRgb(255, 200, 200));

            if (deadline == today)
                return new SolidColorBrush(Color.FromRgb(255, 230, 180));

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
