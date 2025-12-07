using ExpensesAndStuff.Models;

namespace ExpensesAndStuff.ViewModels
{
    public class IncomeItemViewModel : ViewModelBase
    {
        private readonly Income _model;

        public IncomeItemViewModel(Income model)
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

        public IncomeCategory IncomeCategory
        {
            get => _model.IncomeCategory;
            set
            {
                if (_model.IncomeCategory != value)
                {
                    _model.IncomeCategory = value;
                    RaisePropertyChanged(nameof(IncomeCategory));
                }
            }
        }

        public IncomeRecurrence IncomeRecurrence
        {
            get => _model.IncomeRecurrence;
            set
            {
                if (_model.IncomeRecurrence != value)
                {
                    _model.IncomeRecurrence = value;
                    RaisePropertyChanged(nameof(IncomeRecurrence));
                }
            }
        }
    }
}
