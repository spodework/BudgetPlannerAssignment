using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ExpensesAndStuff
{
    public partial class SalaryCalc : Window
    {
        public SalaryCalc()
        {
            InitializeComponent();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // If parameter is "Invert", flip the logic
                if (parameter?.ToString() == "Invert")
                    boolValue = !boolValue;

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
