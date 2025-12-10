using ExpensesAndStuff.Command;
using ExpensesAndStuff.Data;
using ExpensesAndStuff.Models;
using System.Windows.Input;

namespace ExpensesAndStuff.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        private string _userName;
        private decimal _yearlySalary;
        private int _yearlyWorkHours;

        private UserService _userService;
        public ICommand UpdateCommand { get; }
        public UserViewModel(UserService userService)
        {
            _userService = userService;
            _ = LoadUser();
            UpdateCommand = new DelegateCommand(UpdateUser);
        }

        private async void UpdateUser(object? parameter)
        {
            var user = await _userService.GetUserAsync(1);

            user.YearlySalary = YearlySalary;
            user.YearlyWorkHours = YearlyWorkHours;

            await _userService.UpdateUserAsync(user);
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RaisePropertyChanged();
            }
        }

        public decimal YearlySalary
        {
            get => _yearlySalary;
            set
            {
                _yearlySalary = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MonthlySalary));
                RaisePropertyChanged(nameof(YearlyWorkHours));
                RaisePropertyChanged(nameof(HourlyWage));
            }
        }

        public int YearlyWorkHours
        {
            get => _yearlyWorkHours;
            set
            {
                _yearlyWorkHours = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MonthlySalary));
                RaisePropertyChanged(nameof(YearlySalary));
                RaisePropertyChanged(nameof(HourlyWage));
            }
        }
        public decimal MonthlySalary => YearlySalary / 12;

        public decimal HourlyWage => YearlySalary / YearlyWorkHours;

        public async Task LoadUser()
        {
            var user = await _userService.GetUserAsync(1);

            if (user == null)
            {
                user = await _userService.AddAsync(new User() { Id = 1, Name = "AppUser", YearlySalary = 0, YearlyWorkHours = 0 });
            }

            UserName = user.Name;
            YearlySalary = user.YearlySalary;
            YearlyWorkHours = user.YearlyWorkHours;
        }


    }
}
