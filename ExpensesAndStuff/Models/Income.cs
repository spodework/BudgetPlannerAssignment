namespace ExpensesAndStuff.Models
{
    public enum IncomeRecurrence
    {
        OneTime,
        Monthly,
        Yearly,
    }

    public enum IncomeCategory
    {
        Uncategorized,
        Benefits,
        Hobbies,
        MonthlySalary // not shown in combobox to user 
    }
    public class Income
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }



        //public int IncomeCategoryId { get; set; }
        //public string IncomeCategory { get; set; }
        public IncomeRecurrence IncomeRecurrence { get; set; } = IncomeRecurrence.OneTime;
        public IncomeCategory IncomeCategory{ get; set; } = IncomeCategory.Uncategorized;


    }
}
