namespace ExpensesAndStuff.ViewModels
{
    public class TransactionItemViewModel : ViewModelBase
    {
        protected decimal _amount;
        protected DateTime _date;
        protected int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged();
            }
        }

        public decimal Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                RaisePropertyChanged(nameof(Amount));
            }
        }

        public DateTime Date
        {
            get { return _date.Date; }
            set
            {
                _date = value;
                RaisePropertyChanged();
            }
        }
    }
}
