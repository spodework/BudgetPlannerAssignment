using ExpensesAndStuff;
using ExpensesAndStuff.Command;
using ExpensesAndStuff.Interfaces;
using ExpensesAndStuff.Models;
using ExpensesAndStuff.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace IncomesAndStuff.ViewModels
{
    public class IncomeViewModel : ViewModelBase
    {
        private readonly IncomeService _incomeService;
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

        public IncomeViewModel(IncomeService incomeService)
        {            
            _incomeService = incomeService;

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
                    foreach (var expense in _incomeItems)
                        expense.PropertyChanged -= OnExpensePropertyChanged;

                    _incomeItems.CollectionChanged -= OnExpensesCollectionChanged;
                }

                _incomeItems = value;

                if (_incomeItems != null)
                {
                    // Subscribe to new items
                    foreach (var expense in _incomeItems)
                        expense.PropertyChanged += OnExpensePropertyChanged;

                    _incomeItems.CollectionChanged += OnExpensesCollectionChanged;
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

        private async void HandleMonthlySalaryChange()
        {
            var currentMonthly = await _incomeService.FindIncomeByCategoryAsync(IncomeCategory.MonthlySalary);

            if (currentMonthly != null)
            {
                await _incomeService.DeleteIncomeAsync(currentMonthly.Id);

                await LoadIncomes();
            }

            Income income = new()
            {
                IncomeCategory = IncomeCategory.MonthlySalary,
                Amount = _monthlySalaryResult,
                Date = DateTime.Now,
                IncomeRecurrence = IncomeRecurrence.Monthly
            };

            await _incomeService.AddAsync(income);

            var incomeVM = new IncomeItemViewModel(income);
            IncomeItems.Add(incomeVM);
            SelectedIncome = incomeVM;
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
            var salaryVM = new MonthlySalaryViewModel();
            var window = new SalaryCalc
            {
                DataContext = salaryVM
            };

            window.Closed += (s, e) =>
            {
                _monthlySalaryResult = salaryVM.MonthlySalary;
                _hourlySalaryIncome = salaryVM.HourlyWage;

                // todo: do better}
                if (MonthlySalaryResult > 0)
                {
                    MessageBox.Show("Salary has been updated!");
                    HandleMonthlySalaryChange();
                }
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

                foreach (var t in incomes)
                {
                    Debug.WriteLine($"{t.Amount} -- {t.IncomeCategory}");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading incomes: " + ex.Message);
            }
        }

        private void OnExpensePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //RaisePropertyChanged(nameof(TotalAmount));
            RaisePropertyChanged(nameof(NextMonthIncome));
        }

        private void OnExpensesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Handle items added
            if (e.NewItems != null)
                foreach (ViewModelBase item in e.NewItems)
                    item.PropertyChanged += OnExpensePropertyChanged;

            // Handle items removed
            if (e.OldItems != null)
                foreach (ViewModelBase item in e.OldItems)
                    item.PropertyChanged -= OnExpensePropertyChanged;

            //RaisePropertyChanged(nameof(TotalAmount));
            RaisePropertyChanged(nameof(NextMonthIncome));
        }
    }
}





