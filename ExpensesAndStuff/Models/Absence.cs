namespace ExpensesAndStuff.Models
{
    public enum AbsenceType
    {
        VAB,
        Sickness
    }
    public class Absence
    {
        public int Id { get; set; }
        public AbsenceType Type { get; set; }
        public DateTime Date { get; set; }
    }
}
