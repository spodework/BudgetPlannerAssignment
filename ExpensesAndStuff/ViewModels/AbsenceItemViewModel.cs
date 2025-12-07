using ExpensesAndStuff.Models;

namespace ExpensesAndStuff.ViewModels
{
    public class AbsenceItemViewModel : ViewModelBase
    {
        private readonly Absence _model;
        public AbsenceItemViewModel(Absence model)
        {
            _model = model;
        }

        public int Id
        {
            get => _model.Id;
            set
            {
                if (_model.Id != value)
                {
                    _model.Id = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AbsenceType Type
        {
            get => _model.Type;
            set
            {
                if (_model.Type != value)
                {
                    _model.Type = value;
                    RaisePropertyChanged();
                }
            }
        }

        public DateTime Date
        {
            get => _model.Date;
            set
            {
                if (_model.Date != value)
                {
                    _model.Date = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
