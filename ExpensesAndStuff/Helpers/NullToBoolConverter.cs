using System.Globalization;

using System.Windows.Data;

namespace ExpensesAndStuff.Helpers
{

    // used to disable/enable textboxes in xaml

    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
