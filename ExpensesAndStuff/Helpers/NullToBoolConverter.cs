using System.Globalization;

using System.Windows.Data;

namespace ExpensesAndStuff.Helpers
{
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If SelectedExpense is not null, return true (enable the TextBox)
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not needed for this case
            return null;
        }
    }
}
