using ExpensesAndStuff.Command;
using ExpensesAndStuff.Interfaces;
using ExpensesAndStuff.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace ExpensesAndStuff.ViewModels
{
    public class ExpenseViewModel : ViewModelBase
    {
        private readonly ExpenseService _expenseService;

        private ObservableCollection<ExpenseItemViewModel> _expenses = new();

        // full row
        private ExpenseItemViewModel? _selectedExpense;

        // math math math 
        private decimal _forecastedAmount;
        private decimal _totalAmount;

        // for showing enums in dropdown
        public ExpenseRecurrence[] ExpenseRecurrenceArray => Enum.GetValues<ExpenseRecurrence>();
        public ExpenseCategory[] ExpenseCategoryArray => Enum.GetValues<ExpenseCategory>();

        // CONSTRUCTOR
        public ExpenseViewModel(ExpenseService expenseService)//FinanceService financeService)
        {
            _expenseService = expenseService;

            _ = LoadExpenses();
            AddCommand = new DelegateCommand(AddExpense, CanAdd);
            DeleteCommand = new DelegateCommand(DeleteExpense, CanDelete);

            CalculateTotalAmount();
            ForecastNextMonth();

            // Recalculate sum when collection changes
            Expenses.CollectionChanged += (s, e) => CalculateTotalAmount();

            SaveCommand = new DelegateCommand(SaveChanges);
        }

        public ObservableCollection<ExpenseItemViewModel> Expenses
        {
            get { return _expenses; }
            set
            {
                _expenses = value;
                RaisePropertyChanged();
                ForecastNextMonth();
            }
        }

        public decimal ForecastedAmount
        {
            get => _forecastedAmount;
            set
            {
                _forecastedAmount = value;
                RaisePropertyChanged(nameof(ForecastedAmount));
            }
        }

        // Call this method to forecast the next month's expense based on the current list
        private void ForecastNextMonth()
        {
            if (Expenses != null && Expenses.Count > 0)
            {
                var forecast = new ExpenseForecast();
                ForecastedAmount = ExpenseForecast.ForecastNextMonth(
                        Expenses.Select(e => new Expense
                        {
                            Amount = e.Amount,
                            ExpenseRecurrence = e.ExpenseRecurrence,
                            // copy other needed fields
                        }).ToList(),
                    DateTime.Now.Month);
            }
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand DeleteCommand { get; }

        public DelegateCommand SaveCommand { get; }

        public ExpenseItemViewModel? SelectedExpense
        {
            get { return _selectedExpense; }
            set
            {
                if (_selectedExpense != value)
                {
                    _selectedExpense = value;
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
            TotalAmount = Expenses.Sum(exp => exp.Amount);
        }

        private async void AddExpense(object? parameter)
        {
            Expense expense = new()
            {
                ExpenseCategory = ExpenseCategory.Uncategorized,
                Amount = 0,
                Date = DateTime.Now,
                ExpenseRecurrence = ExpenseRecurrence.OneTime
            };
            await _expenseService.AddAsync(expense);

            var expenseVM = new ExpenseItemViewModel(expense);
            Expenses.Add(expenseVM);
            SelectedExpense = expenseVM;
        }

        private async void DeleteExpense(object? parameter)
        {
            if (SelectedExpense is null) return;

            // Call the service to delete the expense from the database
            bool isDeleted = await _expenseService.DeleteExpenseAsync(SelectedExpense.Id);

            if (isDeleted)
            {
                // If the deletion was successful, remove the item from ObservableCollection
                Expenses.Remove(SelectedExpense);
            }
            else
            {
                // If not, show an error message
                MessageBox.Show("Expense not found.");
            }

        }

        private bool CanDelete(object? parameter) => true; //todo
        private bool CanAdd(object? parameter) => true; //todo


        private async void SaveChanges(object? parameter)
        {
            if (SelectedExpense is null) { return; }

            var expenseEntity = new Expense
            {
                Id = SelectedExpense.Id,
                Amount = SelectedExpense.Amount,
                ExpenseCategory = SelectedExpense.ExpenseCategory,
                ExpenseRecurrence = SelectedExpense.ExpenseRecurrence,
            };

            if (SelectedExpense.Id == 0)
            {
                await _expenseService.AddAsync(expenseEntity);
            }
            else
            {
                await _expenseService.UpdateAsync(expenseEntity);
            }

            await _expenseService.SaveChangesAsync();

        }

        public async Task LoadExpenses()
        {
            try
            {
                var expenses = await _expenseService.GetExpensesAsync();

                // convert to ExpenseItemViewModel for rendering
                Expenses = new ObservableCollection<ExpenseItemViewModel>(
                    expenses.Select(expense => new ExpenseItemViewModel(expense))
                );

                foreach (var t in expenses)
                {
                    Debug.WriteLine($"{t.Amount} -- {t.ExpenseCategory}");
                }

            }
            catch (Exception ex)
            {
                // todo handle errors
            }
        }
    }
}
