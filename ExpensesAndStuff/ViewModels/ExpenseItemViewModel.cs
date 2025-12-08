using ExpensesAndStuff.Models;

namespace ExpensesAndStuff.ViewModels
{
    public class ExpenseItemViewModel : ViewModelBase
    {
        private readonly Expense _model;
        public ExpenseItemViewModel(Expense model)
        {
            _model = model;
        }

        public int Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                RaisePropertyChanged();
            }
        }

        public decimal Amount
        {
            get { return _model.Amount; }
            set
            {
                _model.Amount = value;
                RaisePropertyChanged(nameof(Amount));
            }
        }

        public DateTime Date
        {
            get { return _model.Date; }
            set
            {
                _model.Date = value;
                RaisePropertyChanged();
            }
        }

        public ExpenseRecurrence ExpenseRecurrence
        {
            get => _model.ExpenseRecurrence;
            set
            {
                if (_model.ExpenseRecurrence != value)
                {
                    _model.ExpenseRecurrence = value;
                    RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }
    }

}

