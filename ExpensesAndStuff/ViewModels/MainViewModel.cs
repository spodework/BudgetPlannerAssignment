using ExpensesAndStuff.Interfaces;
using IncomesAndStuff.ViewModels;

namespace ExpensesAndStuff.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private DateTime _currentDate;

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    RaisePropertyChanged(nameof(CurrentDate));
                    RaisePropertyChanged(nameof(CurrentDateString));
                }
            }
        }

        public string CurrentDateString => $"Today is {CurrentDate:dd MMMM yyyy}";

        public ExpenseViewModel ExpenseVM { get; }
        public IncomeViewModel IncomeVM { get; }

        private readonly IncomeService _incomeService;
        private readonly ExpenseService _expenseService;

        public MainViewModel(ExpenseService expenseService, IncomeService incomeService)
        {
            _incomeService = incomeService;
            _expenseService = expenseService;

            _currentDate = DateTime.Now;
            ExpenseVM = new ExpenseViewModel(_expenseService);
            IncomeVM = new IncomeViewModel(_incomeService);
        }
    }
}
