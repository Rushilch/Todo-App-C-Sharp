using System;
using System.Globalization;
using System.Windows.Data;

namespace ProductivityApp.Converters
{
    public class DateEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value: SelectedDate (from DataContext)
            // parameter: current item (DateTime) passed via RelativeSource Binding
            if (value is DateTime selected && parameter is DateTime current)
            {
                return selected.Date == current.Date;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
