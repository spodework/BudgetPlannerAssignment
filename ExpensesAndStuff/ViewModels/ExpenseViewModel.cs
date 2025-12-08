using ExpensesAndStuff.Command;
using ExpensesAndStuff.Interfaces;
using ExpensesAndStuff.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace ExpensesAndStuff.ViewModels
{
    public class ExpenseViewModel : ViewModelBase
    {
        private ObservableCollection<ExpenseItemViewModel> _expenseItems;
        private ExpenseItemViewModel? _selectedExpense;

        private readonly ExpenseService _expenseService;

        // for showing enums in dropdown
        public ExpenseRecurrence[] ExpenseRecurrenceArray => Enum.GetValues<ExpenseRecurrence>();
        public ExpenseCategory[] ExpenseCategoryArray => Enum.GetValues<ExpenseCategory>();

        // CONSTRUCTOR
        public ExpenseViewModel(ExpenseService expenseService)
        {
            _expenseService = expenseService;
            _ = LoadExpensesAsync();

            AddCommand = new DelegateCommand(AddExpense);
            DeleteCommand = new DelegateCommand(DeleteExpense, CanDelete);
            SaveCommand = new DelegateCommand(SaveChanges);
        }

        public ObservableCollection<ExpenseItemViewModel> ExpenseItems
        {
            get { return _expenseItems; }
            set
            {
                if (_expenseItems != null)
                {
                    // Unsubscribe from old items
                    foreach (var expense in _expenseItems)
                        expense.PropertyChanged -= OnExpensePropertyChanged;

                    _expenseItems.CollectionChanged -= OnExpenseItemsCollectionChanged;
                }

                _expenseItems = value;

                if (_expenseItems != null)
                {
                    // Subscribe to new items
                    foreach (var expense in _expenseItems)
                        expense.PropertyChanged += OnExpensePropertyChanged;

                    _expenseItems.CollectionChanged += OnExpenseItemsCollectionChanged;
                }
            }
        }

        public decimal NextMonthExpenses => ForecastNextMonth();

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

        public decimal TotalAmount => ExpenseItems?.Sum(e => e.Amount) ?? 0;


        private async void AddExpense(object? parameter)
        {
            Expense expense = new()
            {
                ExpenseCategory = ExpenseCategory.Uncategorized,
                Amount = 500,
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

        public async Task LoadExpensesAsync()
        {
            try
            {
                var expenses = await _expenseService.GetExpensesAsync();

                // convert to ExpenseItemViewModel for rendering
                ExpenseItems = new ObservableCollection<ExpenseItemViewModel>(
                    expenses.Select(expense => new ExpenseItemViewModel(expense))
                );
            }
            catch (Exception ex)
            {
                // todo handle errors
            }
        }


        public decimal ForecastNextMonth()
        {
            var thisMonth = DateTime.Now.Month;

            if (ExpenseItems == null || ExpenseItems.Count == 0) return 0;

            int nextMonth = (thisMonth % 12) + 1;

            decimal forecastedAmount = 0;

            foreach (var expense in ExpenseItems)
            {
                forecastedAmount += GetExpenseForNextMonth(new Expense() { Amount = expense.Amount, ExpenseRecurrence = expense.ExpenseRecurrence }, nextMonth);
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

        private void OnExpensePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TotalAmount));
            RaisePropertyChanged(nameof(NextMonthExpenses));
        }

        private void OnExpenseItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Handle items added
            if (e.NewItems != null)
                foreach (ViewModelBase item in e.NewItems)
                    item.PropertyChanged += OnExpensePropertyChanged;

            // Handle items removed
            if (e.OldItems != null)
                foreach (ViewModelBase item in e.OldItems)
                    item.PropertyChanged -= OnExpensePropertyChanged;

            RaisePropertyChanged(nameof(TotalAmount));
            RaisePropertyChanged(nameof(NextMonthExpenses));
        }
    }
}
