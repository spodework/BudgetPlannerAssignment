using ExpensesAndStuff.Command;
using System.Windows.Input;

namespace ExpensesAndStuff.ViewModels
{
    public class MonthlySalaryViewModel : ViewModelBase
    {
        public List<string> CalculationModes { get; } = new() { "Hourly", "Yearly" };

        private string _selectedMode;
        public string SelectedMode
        {
            get => _selectedMode;
            set
            {
                _selectedMode = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsHourlyMode));
                RaisePropertyChanged(nameof(IsYearlyMode));
            }
        }

        public bool IsHourlyMode => SelectedMode == "Hourly";
        public bool IsYearlyMode => SelectedMode == "Yearly";

        public double YearlyHours { get; set; }
        public double HourlyWage { get; set; }
        public double YearlySalary { get; set; }

        private double _monthlySalary;
        public double MonthlySalary
        {
            get => _monthlySalary;
            set
            {
                _monthlySalary = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CalculateCommand { get; }

        public MonthlySalaryViewModel()
        {
            CalculateCommand = new DelegateCommand(Calculate);
            SelectedMode = CalculationModes.First();
        }

        private void Calculate(object? parameter)
        {
            if (IsHourlyMode)
                MonthlySalary = YearlyHours * HourlyWage / 12 ;
            else if (IsYearlyMode)
                MonthlySalary = YearlySalary / 12;
        }
    }
}
