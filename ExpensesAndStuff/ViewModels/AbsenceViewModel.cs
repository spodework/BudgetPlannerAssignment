using ExpensesAndStuff.Command;
using ExpensesAndStuff.Interfaces;
using ExpensesAndStuff.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace ExpensesAndStuff.ViewModels
{
    public class AbsenceViewModel : ViewModelBase
    {
        private ObservableCollection<AbsenceItemViewModel> _absenceItems = new();

        private readonly AbsenceService _absenceService;
        private AbsenceItemViewModel? _selectedAbsence;

        public int AbsenceDaysThisMonth => GetAbsenceDaysForMonth();

        public AbsenceType[] AbsenceTypeArray => Enum.GetValues<AbsenceType>();

        public DelegateCommand AddCommand { get; }
        public DelegateCommand DeleteCommand { get; }

        public DelegateCommand SaveCommand { get; }

        // CONSTRUCTOR
        public AbsenceViewModel(AbsenceService absenceService)
        {
            _absenceService = absenceService;

            _ = LoadAbsences();
            AddCommand = new DelegateCommand(AddAbsence);

            DeleteCommand = new DelegateCommand(DeleteAbsence, CanDelete);
            SaveCommand = new DelegateCommand(SaveChanges);
        }

        private bool CanDelete(object? parameter) => SelectedAbsence is not null;
        public ObservableCollection<AbsenceItemViewModel> AbsenceItems
        {
            get { return _absenceItems; }
            set
            {
                if (_absenceItems != value)
                {
                    _absenceItems = value;
                    RaisePropertyChanged();
                }
            }
        }


        public AbsenceItemViewModel? SelectedAbsence
        {
            get { return _selectedAbsence; }
            set
            {
                if (_selectedAbsence != value)
                {
                    _selectedAbsence = value;
                    RaisePropertyChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async void DeleteAbsence(object? parameter)
        {
            if (SelectedAbsence is null) return;

            bool isDeleted = await _absenceService.DeleteAbsenceAsync(SelectedAbsence.Id);

            if (isDeleted)
            {
                // If the deletion was successful, remove the item from ObservableCollection
                AbsenceItems.Remove(SelectedAbsence);
            }
            else
            {
                MessageBox.Show("Absence not found.");
            }

        }

        private async void AddAbsence(object? parameter)
        {
            Absence absence = new()
            {
                Type = AbsenceType.VAB,
                Date = DateTime.Now
            };
            await _absenceService.AddAsync(absence);

            var absenceVM = new AbsenceItemViewModel(absence);
            AbsenceItems.Add(absenceVM);
            SelectedAbsence = absenceVM;
        }
        private async void SaveChanges(object? parameter)
        {
            await _absenceService.SaveChangesAsync();
        }

        public async Task LoadAbsences()
        {
            try
            {
                var expenses = await _absenceService.GetAbsencesAsync();

                // convert to AbsenceItemViewModel for rendering
                AbsenceItems = new ObservableCollection<AbsenceItemViewModel>(
                    expenses.Select(expense => new AbsenceItemViewModel(expense))
                );
            }
            catch (Exception ex)
            {
                // todo handle errors
            }
        }

        public int GetAbsenceDaysForMonth()
        {
            var curDate = DateTime.Now;
            var thisMonth = curDate.Month;
            var thisYear = curDate.Year;

            var absenceDaysInMonth = AbsenceItems
                .Where(item => item.Date.Month == thisMonth && item.Date.Year == thisYear)
                .Count();

            return absenceDaysInMonth;
        }

    }
}
