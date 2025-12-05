using ExpensesAndStuff;
using ExpensesAndStuff.Command;
using ExpensesAndStuff.Interfaces;
using ExpensesAndStuff.Models;
using ExpensesAndStuff.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace IncomesAndStuff.ViewModels
{
    public class IncomeViewModel : ViewModelBase
    {
        private readonly IncomeService _incomeService;
        private ObservableCollection<IncomeCategory> _incomeCategories;
        private ObservableCollection<IncomeItemViewModel> _incomes = new();

        // full row
        private IncomeItemViewModel? _selectedIncome;

        private double _monthlySalaryResult;
        public double MonthlySalaryResult
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

        // math math math 
        private decimal _totalAmount;

        // for showing enums in dropdown
        public IncomeRecurrence[] IncomeRecurrenceArray => Enum.GetValues<IncomeRecurrence>();
        public IncomeCategory[] IncomeCategoryArray => Enum.GetValues<IncomeCategory>()
            .Where(c => c != IncomeCategory.MonthlySalary)
            .ToArray();

        // CONSTRUCTOR

        public IncomeViewModel(IncomeService incomeService)
        {
            _incomeService = incomeService;

            _ = LoadIncomes();
            AddCommand = new DelegateCommand(AddIncome, CanAdd);
            DeleteCommand = new DelegateCommand(DeleteIncome, CanDelete);

            CalculateTotalAmount();

            // Recalculate sum when collection changes
            Incomes.CollectionChanged += (s, e) => CalculateTotalAmount();

            SaveCommand = new DelegateCommand(SaveChanges);

            CalculateSalaryCommand = new DelegateCommand(OpenSalaryCalc);
        }

        public ObservableCollection<IncomeItemViewModel> Incomes
        {
            get { return _incomes; }
            set
            {
                _incomes = value;
                RaisePropertyChanged();
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

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                if (_totalAmount != value)
                {
                    _totalAmount = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = Incomes.Sum(exp => exp.Amount);
        }

        public ObservableCollection<IncomeCategory> IncomeCategories
        {
            get => _incomeCategories;
            set
            {
                if (_incomeCategories != value)
                {
                    _incomeCategories = value;
                    RaisePropertyChanged(nameof(IncomeCategories));
                }
            }
        }

        private async void AddIncome(object? parameter)
        {
            Income income = new()
            {
                IncomeCategory = IncomeCategory.Uncategorized,
                Amount = 0,
                Date = DateTime.Now,
                IncomeRecurrence = IncomeRecurrence.OneTime
            };
            await _incomeService.AddAsync(income);

            var incomeVM = new IncomeItemViewModel(income);
            Incomes.Add(incomeVM);
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
                Amount = (decimal)_monthlySalaryResult, // maybe not great to cast here
                Date = DateTime.Now,
                IncomeRecurrence = IncomeRecurrence.OneTime
            };

            await _incomeService.AddAsync(income);

            var incomeVM = new IncomeItemViewModel(income);
            Incomes.Add(incomeVM);
            SelectedIncome = incomeVM;
        }


        private async void DeleteIncome(object? parameter)
        {
            if (SelectedIncome is null) return;

            bool isDeleted = await _incomeService.DeleteIncomeAsync(SelectedIncome.Id);

            if (isDeleted)
            {
                Incomes.Remove(SelectedIncome);
            }
            else
            {
                MessageBox.Show("Income not found.");
            }

        }

        private bool CanDelete(object? parameter) => true; //todo better check
        private bool CanAdd(object? parameter) => true; //todo better check

        private async void SaveChanges(object? parameter)
        {
            if (SelectedIncome is null) { return; }

            var income = new Income
            {
                Id = SelectedIncome.Id,
                Amount = SelectedIncome.Amount,
                IncomeCategory = SelectedIncome.IncomeCategory,
                IncomeRecurrence = SelectedIncome.IncomeRecurrence
            };

            if (SelectedIncome.Id == 0)
            {
                await _incomeService.AddAsync(income);
            }
            else
            {
                await _incomeService.UpdateAsync(income);
            }

            await _incomeService.SaveChangesAsync();

        }

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
                MessageBox.Show("Salary has been updated!");


                if (MonthlySalaryResult > 0) HandleMonthlySalaryChange(); // todo: do better
            };

            window.Show();

        }


        public async Task LoadIncomes()
        {
            try
            {
                var incomes = await _incomeService.GetIncomesAsync();
               
                Incomes = new ObservableCollection<IncomeItemViewModel>(
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
    }
}





