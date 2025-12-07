using ExpensesAndStuff.Command;
using ExpensesAndStuff.Helpers;
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
        private ObservableCollection<ExpenseItemViewModel> _expenseItems = new();
        private ExpenseItemViewModel? _selectedExpense;

        private readonly ExpenseService _expenseService;

        private decimal _nextMonthExpense;
        private decimal _totalAmount;

        // for showing enums in dropdown
        public ExpenseRecurrence[] ExpenseRecurrenceArray => Enum.GetValues<ExpenseRecurrence>();
        public ExpenseCategory[] ExpenseCategoryArray => Enum.GetValues<ExpenseCategory>();

        // CONSTRUCTOR
        public ExpenseViewModel(ExpenseService expenseService)
        {
            _expenseService = expenseService;

            _ = LoadExpenses();
  
            CalculateTotalAmount();
            ForecastNextMonthExpenses();

            ExpenseItems.CollectionChanged += (s, e) => {
                CalculateTotalAmount();
                ForecastNextMonthExpenses();
            };

            AddCommand = new DelegateCommand(AddExpense);
            DeleteCommand = new DelegateCommand(DeleteExpense, CanDelete);
            SaveCommand = new DelegateCommand(SaveChanges);
        }

        public ObservableCollection<ExpenseItemViewModel> ExpenseItems
        {
            get { return _expenseItems; }
            set
            {
                if (_expenseItems != value)
                {
                    _expenseItems = value;
                    RaisePropertyChanged();
                    ForecastNextMonthExpenses();
                }
            }
        }

        public decimal NextMonthExpense
        {
            get => _nextMonthExpense;
            set
            {
                _nextMonthExpense = value;
                RaisePropertyChanged(nameof(NextMonthExpense));
            }
        }

        private void ForecastNextMonthExpenses()
        {
            if (ExpenseItems != null && ExpenseItems.Count > 0)
            {
                NextMonthExpense = ForecastNextMonth(
                        ExpenseItems.Select(e => new Expense
                        {
                            Amount = e.Amount,
                            ExpenseRecurrence = e.ExpenseRecurrence,
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
                    CalculateTotalAmount();
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
            TotalAmount = ExpenseItems.Sum(exp => exp.Amount);
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
            ExpenseItems.Add(expenseVM);
            SelectedExpense = expenseVM;
        }

        private async void DeleteExpense(object? parameter)
        {
            if (SelectedExpense is null) return;

            bool isDeleted = await _expenseService.DeleteExpenseAsync(SelectedExpense.Id);

            if (isDeleted)
            {
                ExpenseItems.Remove(SelectedExpense);
            }
            else
            {
                MessageBox.Show("Expense not found.");
            }

        }

        private bool CanDelete(object? parameter) => SelectedExpense is not null; //todo

        private async void SaveChanges(object? parameter)
        {
            await _expenseService.SaveChangesAsync();

            MessageBox.Show("Changes have been saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public async Task LoadExpenses()
        {
            try
            {
                var expenses = await _expenseService.GetExpensesAsync();

                // convert to ExpenseItemViewModel for rendering
                ExpenseItems = new ObservableCollection<ExpenseItemViewModel>(
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


        public static decimal ForecastNextMonth(List<Expense> expensesList, int thisMonth)
        {
            if (expensesList == null || expensesList.Count == 0)
                return 0;


            int nextMonth = (thisMonth % 12) + 1;

            decimal forecastedAmount = 0;
            foreach (var expense in expensesList)
            {
                forecastedAmount += GetExpenseForNextMonth(expense, nextMonth);
            }

            return forecastedAmount;
        }

        public static decimal GetExpenseForNextMonth(Expense expense, int nextMonth)
        {
            switch (expense.ExpenseRecurrence)
            {
                case ExpenseRecurrence.OneTime:
                    return 0; // skip

                case ExpenseRecurrence.Monthly:
                    return expense.Amount; // always include

                case ExpenseRecurrence.YearlyJanuary:
                    return nextMonth == 1 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyFebruary:
                    return nextMonth == 2 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyMarch:
                    return nextMonth == 3 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyApril:
                    return nextMonth == 4 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyMay:
                    return nextMonth == 5 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyJune:
                    return nextMonth == 6 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyJuly:
                    return nextMonth == 7 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyAugust:
                    return nextMonth == 8 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlySeptember:
                    return nextMonth == 9 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyOctober:
                    return nextMonth == 10 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyNovember:
                    return nextMonth == 11 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyDecember:
                    return nextMonth == 12 ? expense.Amount : 0;

                default:
                    return 0;
            }
        }

    }
}
