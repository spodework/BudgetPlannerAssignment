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
        private readonly AbsenceService _absenceService;

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

        private ObservableCollection<IncomeCategory> _incomeCategories;

        private ObservableCollection<IncomeItemViewModel> _incomeItems = new();

        private IncomeItemViewModel? _selectedIncome;

        // for showing enums in dropdown
        public IncomeRecurrence[] IncomeRecurrenceArray => Enum.GetValues<IncomeRecurrence>();
        public IncomeCategory[] IncomeCategoryArray => Enum.GetValues<IncomeCategory>()
            .Where(c => c != IncomeCategory.MonthlySalary)
            .ToArray();

        public async Task<int> GetThisMonthAbsencesAsync()
        {
            var absences = await _absenceService.GetThisMonthAbsencesAsync();
            var filtered = absences.Where(item => item.Type == AbsenceType.VAB);
            return filtered.Count();
        }

        public async Task<decimal> GetNextMonthIncomeAsync()
        {
            var monthlyIncomeItems = IncomeItems
                .Where(item => item.IncomeCategory == IncomeCategory.MonthlySalary)
                .ToList();

            var absences = await _absenceService.GetThisMonthAbsencesAsync();
            var filtered = absences.Where(item => item.Type == AbsenceType.VAB);

            var absentDaysCount = filtered.Count();

            decimal monthlyIncomeSum = monthlyIncomeItems.Sum(exp => exp.Amount);

            decimal daysWorth = (8 * HourlySalary);

            decimal monthlyIncomeSumMinusSickdays = monthlyIncomeSum - ((8 * HourlySalary) * absentDaysCount);

            decimal monthlyIncomeSumMinusSickdaysPlusBenefits = monthlyIncomeSumMinusSickdays + ((8 * HourlySalary) * 0.8m);
            
            return monthlyIncomeSumMinusSickdaysPlusBenefits;
        }

        // CONSTRUCTOR

        public IncomeViewModel(IncomeService incomeService, UserViewModel userViewModel, AbsenceService absenceService)
        {

            _incomeService = incomeService;
            _userViewModel = userViewModel;
            _absenceService = absenceService;
            _ = LoadIncomes();
            _ = LoadNextMonthIncomeAsync();
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
        public decimal HourlySalary => _userViewModel.HourlyWage;
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

        private decimal _nextMonthIncome;
        public decimal NextMonthIncome
        {
            get
            {
                return _nextMonthIncome;
            }
        }

        private void OpenSalaryCalc(object? parameter)
        {
            var window = new SalaryCalc
            {
                DataContext = _userViewModel
            };

            window.Closed += async (s, e) =>
            {
                await UpdateMonthlyIncomeInDb();
                await LoadNextMonthIncomeAsync();
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
            RaisePropertyChanged(nameof(HourlySalary));
            LoadNextMonthIncomeAsync();
        }

        public async Task LoadNextMonthIncomeAsync()
        {
            _nextMonthIncome = await GetNextMonthIncomeAsync();
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
            RaisePropertyChanged(nameof(HourlySalary));
            LoadNextMonthIncomeAsync();

        }
    }
}





