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

        public int YearlyHours { get; set; }
        public decimal HourlyWage { get; set; }
        public decimal YearlySalary { get; set; }

        private decimal _monthlySalary;
        public decimal MonthlySalary
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
            MonthlySalary = YearlySalary / 12;
            HourlyWage = YearlySalary / YearlyHours;
            //MonthlySalary = YearlyHours * HourlyWage / 12;
            //if (IsHourlyMode)
            //MonthlySalary = YearlyHours * HourlyWage / 12 ;
            //else if (IsYearlyMode)
            //    MonthlySalary = YearlySalary / 12;
        }
    }
}
