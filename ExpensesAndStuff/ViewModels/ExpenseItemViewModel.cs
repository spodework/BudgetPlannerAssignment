using ExpensesAndStuff.Models;

namespace ExpensesAndStuff.ViewModels
{
    public class ExpenseItemViewModel : TransactionItemViewModel
    {
        private readonly Expense _model;
        public ExpenseItemViewModel(Expense model)
        {
            _model = model;
            _amount = model.Amount;
            _date = model.Date;
            _id = model.Id;
        }

        public ExpenseRecurrence ExpenseRecurrence
        {
            get => _model.ExpenseRecurrence;
            set
            {
                if (_model.ExpenseRecurrence != value)
                {
                    _model.ExpenseRecurrence = value;
                    RaisePropertyChanged(nameof(ExpenseRecurrence));

                }
            }
        }

        public ExpenseCategory ExpenseCategory
        {
            get => _model.ExpenseCategory;
            set
            {
                if (_model.ExpenseCategory != value)

                    _model.ExpenseCategory = value;
                RaisePropertyChanged(nameof(ExpenseCategory));
            }
        }
    }

}

