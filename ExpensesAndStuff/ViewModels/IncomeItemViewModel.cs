using ExpensesAndStuff.Models;

namespace ExpensesAndStuff.ViewModels
{
    public class IncomeItemViewModel : TransactionItemViewModel
    {
        private readonly Income model;

        public IncomeItemViewModel(Income model)
        {
            this.model = model;
            _amount = model.Amount;
            _date = model.Date;
            _id = model.Id;
        }
        public IncomeCategory IncomeCategory
        {
            get => model.IncomeCategory;
            set
            {
                if (model.IncomeCategory != value)
                {
                    model.IncomeCategory = value;
                    RaisePropertyChanged(nameof(IncomeCategory));
                }
            }
        }

        public IncomeRecurrence IncomeRecurrence
        {
            get => model.IncomeRecurrence;
            set
            {
                if (model.IncomeRecurrence != value)
                {
                    model.IncomeRecurrence = value;
                    RaisePropertyChanged(nameof(IncomeRecurrence));
                }
            }
        }
    }
}
