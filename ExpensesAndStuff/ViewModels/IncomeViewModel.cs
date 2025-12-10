using ExpensesAndStuff;
using ExpensesAndStuff.Command;
using ExpensesAndStuff.Interfaces;
using ExpensesAndStuff.Models;
using ExpensesAndStuff.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace IncomesAndStuff.ViewModels
{
    public class IncomeViewModel : ViewModelBase
    {
        private readonly IncomeService _incomeService;

        private UserViewModel _userViewModel;
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(UserName);
                }
            }
        }

        private decimal _hourlySalary;
        public decimal HourlySalary
        {
            get { return _hourlySalary; }
            set
            {
                if (_hourlySalary != value)
                {
                    _hourlySalary = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _monthlySalary;
        public int MonthlySalary
        {
            get { return _monthlySalary; }
            set
            {
                if (_monthlySalary != value)
                {
                    _monthlySalary = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _yearlyWorkHours;
        public int YearlyWorkHours
        {
            get { return _yearlyWorkHours; }
            set
            {
                if (_yearlyWorkHours != value)
                {
                    _yearlyWorkHours = value;
                    RaisePropertyChanged();
                }
            }
        }

        private decimal _yearlySalary;
        public decimal YearlySalary
        {
            get { return _yearlySalary; }
            set
            {
                if (_yearlySalary != value)
                {
                    _yearlySalary = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ObservableCollection<IncomeCategory> _incomeCategories;

        private ObservableCollection<IncomeItemViewModel> _incomeItems = new();

        private IncomeItemViewModel? _selectedIncome;

        // math math math 
        private decimal _hourlySalaryIncome;
        private decimal _monthlySalaryResult;

        // for showing enums in dropdown
        public IncomeRecurrence[] IncomeRecurrenceArray => Enum.GetValues<IncomeRecurrence>();
        public IncomeCategory[] IncomeCategoryArray => Enum.GetValues<IncomeCategory>()
            .Where(c => c != IncomeCategory.MonthlySalary)
            .ToArray();

        public decimal MonthlySalaryResult
        {
            get { return _monthlySalaryResult; }
            set
            {
                if (_monthlySalaryResult != value)
                {
                    _monthlySalaryResult = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal HourlySalaryIncome
        {
            get { return _hourlySalaryIncome; }
            set
            {
                if (_hourlySalaryIncome != value)
                {
                    _hourlySalaryIncome = value;
                    RaisePropertyChanged();
                }
            }
        }


        public decimal GetNextMonthIncome()
        {
            var monthlyIncomeItems = IncomeItems
                .Where(item => item.IncomeCategory == IncomeCategory.MonthlySalary)
                .ToList();

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            decimal monthlyIncomeSum = monthlyIncomeItems.Sum(exp => exp.Amount);

            return monthlyIncomeSum;
        }

        // CONSTRUCTOR

        public IncomeViewModel(IncomeService incomeService, UserViewModel userViewModel)
        {
            _incomeService = incomeService;
            _userViewModel = userViewModel;

            _ = LoadIncomes();
            AddCommand = new DelegateCommand(AddIncome);
            DeleteCommand = new DelegateCommand(DeleteIncome, CanDelete);

            SaveCommand = new DelegateCommand(SaveChanges);

            CalculateSalaryCommand = new DelegateCommand(OpenSalaryCalc);
        }

        public ObservableCollection<IncomeItemViewModel> IncomeItems
        {
            get { return _incomeItems; }
            set
            {

                if (_incomeItems != null)
                {
                    // Unsubscribe from old items
                    foreach (var income in _incomeItems)
                        income.PropertyChanged -= OnIncomePropertyChanged;

                    _incomeItems.CollectionChanged -= OnIncomeItemsCollectionChanged;
                }

                _incomeItems = value;
                RaisePropertyChanged(nameof(IncomeItems));


                if (_incomeItems != null)
                {
                    // Subscribe to new items
                    foreach (var income in _incomeItems)
                        income.PropertyChanged += OnIncomePropertyChanged;

                    _incomeItems.CollectionChanged += OnIncomeItemsCollectionChanged;
                }
            }
        }
        public DelegateCommand AddCommand { get; }
        public DelegateCommand DeleteCommand { get; }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CalculateSalaryCommand { get; }

        public IncomeItemViewModel? SelectedIncome
        {
            get { return _selectedIncome; }
            set
            {
                if (_selectedIncome != value)
                {
                    _selectedIncome = value;
                    RaisePropertyChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal TotalIncome => IncomeItems?.Sum(e => e.Amount) ?? 0;


        public ObservableCollection<IncomeCategory> IncomeCategories
        {
            get => _incomeCategories;
            set
            {
                if (_incomeCategories != value)
                {
                    _incomeCategories = value;
                    RaisePropertyChanged();
                }
            }
        }

        private async void AddIncome(object? parameter)
        {
            Income income = new()
            {
                IncomeCategory = IncomeCategory.Uncategorized,
                Amount = 500,
                Date = DateTime.Now,
                IncomeRecurrence = IncomeRecurrence.OneTime
            };
            await _incomeService.AddAsync(income);

            var incomeVM = new IncomeItemViewModel(income);
            IncomeItems.Add(incomeVM);
            SelectedIncome = incomeVM;
        }

        private async Task UpdateMonthlyIncomeInDb()
        {
            var currentMonthly = await _incomeService.FindIncomeByCategoryAsync(IncomeCategory.MonthlySalary);

            if (currentMonthly != null)
            {
                await _incomeService.DeleteIncomeAsync(currentMonthly.Id);
                LoadIncomes();
            }

            Income income = new()
            {
                IncomeCategory = IncomeCategory.MonthlySalary,
                Amount = _userViewModel.MonthlySalary,
                Date = DateTime.Now,
                IncomeRecurrence = IncomeRecurrence.Monthly
            };

            var incomeVM = new IncomeItemViewModel(income);
            IncomeItems.Add(incomeVM);
            SelectedIncome = incomeVM;

            await _incomeService.AddAsync(income);

        }


        private async void DeleteIncome(object? parameter)
        {
            if (SelectedIncome is null) return;

            bool isDeleted = await _incomeService.DeleteIncomeAsync(SelectedIncome.Id);

            if (isDeleted)
            {
                IncomeItems.Remove(SelectedIncome);
            }
            else
            {
                MessageBox.Show("Income not found.");
            }

        }

        private bool CanDelete(object? parameter) => SelectedIncome is not null;

        private async void SaveChanges(object? parameter)
        {
            await _incomeService.SaveChangesAsync();
        }

        public decimal NextMonthIncome => GetNextMonthIncome();


        private void OpenSalaryCalc(object? parameter)
        {
            var window = new SalaryCalc
            {
                DataContext = _userViewModel
            };

            window.Closed += async (s, e) =>
            {
                await UpdateMonthlyIncomeInDb();
            };

            window.Show();
        }

        public async Task LoadIncomes()
        {

            try
            {
                var incomes = await _incomeService.GetIncomesAsync();

                IncomeItems = new ObservableCollection<IncomeItemViewModel>(
                    incomes.Select(income => new IncomeItemViewModel(income))
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading incomes: " + ex.Message);
            }
        }

        private void OnIncomePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TotalIncome));
            RaisePropertyChanged(nameof(NextMonthIncome));
        }

        private void OnIncomeItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Handle items added
            if (e.NewItems != null)
                foreach (ViewModelBase item in e.NewItems)
                    item.PropertyChanged += OnIncomePropertyChanged;

            // Handle items removed
            if (e.OldItems != null)
                foreach (ViewModelBase item in e.OldItems)
                    item.PropertyChanged -= OnIncomePropertyChanged;

            RaisePropertyChanged(nameof(TotalIncome));
            RaisePropertyChanged(nameof(NextMonthIncome));
        }
    }
}





