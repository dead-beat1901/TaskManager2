using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int p)
            {
                switch (p)
                {
                    case 1: return new SolidColorBrush(Color.FromRgb(255, 210, 210));
                    case 2: return new SolidColorBrush(Color.FromRgb(255, 255, 210));
                    case 3: return Brushes.White;
                }
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
