using ExpensesAndStuff.Command;
using ExpensesAndStuff.Interfaces;
using IncomesAndStuff.ViewModels;

namespace ExpensesAndStuff.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private DateTime _currentDate;
        public string CurrentDateString => $"Today is {CurrentDate:dd MMMM yyyy}";

        private IncomeViewModel _incomeViewModel;
        private ExpenseViewModel _expenseViewModel;
        private AbsenceViewModel _absenceViewModel;

        private readonly IncomeService _incomeService;
        private readonly ExpenseService _expenseService;
        private readonly AbsenceService _absenceService;

        //CONSTRUCTOR
        public MainViewModel(ExpenseService expenseService, IncomeService incomeService, AbsenceService absenceService)
        {
            _currentDate = DateTime.Now;

            _incomeService = incomeService;
            _expenseService = expenseService;
            _absenceService = absenceService;
            

            ExpenseVM = new ExpenseViewModel(_expenseService);
            IncomeVM = new IncomeViewModel(_incomeService);
            AbsenceVM = new AbsenceViewModel(_absenceService);
            
        }

        private decimal _nextMonthTotalForecast;
        public decimal NextMonthTotalForecast
        {
            get => _nextMonthTotalForecast;
            set
            {
                _nextMonthTotalForecast = value;
                RaisePropertyChanged(nameof(NextMonthTotalForecast));
            }
        }

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

        public decimal IncomesMinusExpenses
        {
            get
            {
                return IncomeVM.TotalIncome - ExpenseVM.TotalAmount;
            }
        }

        public IncomeViewModel IncomeVM
        {
            get { return _incomeViewModel; }
            set
            {
                if (_incomeViewModel != value)
                {
                    _incomeViewModel = value;
                    RaisePropertyChanged(nameof(IncomeVM));
                    RaisePropertyChanged(nameof(IncomesMinusExpenses)); // Recalculate Total when Income changes
                }
            }
        }

        public ExpenseViewModel ExpenseVM
        {
            get { return _expenseViewModel; }
            set
            {
                if (_expenseViewModel != value)
                {
                    _expenseViewModel = value;
                    RaisePropertyChanged(nameof(ExpenseVM));
                    RaisePropertyChanged(nameof(IncomesMinusExpenses)); // Recalculate Total when Expense changes
                }
            }
        }

        public AbsenceViewModel AbsenceVM
        {
            get { return _absenceViewModel; }
            set
            {
                if (_absenceViewModel != value)
                {
                    _absenceViewModel = value;
                    RaisePropertyChanged(nameof(AbsenceVM));
                }
            }
        }
    }
}
