namespace ExpensesAndStuff.Models
{
    public enum ExpenseRecurrence
    {
        OneTime,
        Monthly,
        YearlyJanuary,
        YearlyFebruary,
        YearlyMarch,
        YearlyApril,
        YearlyMay,
        YearlyJune,
        YearlyJuly,
        YearlyAugust,
        YearlySeptember,
        YearlyOctober,
        YearlyNovember,
        YearlyDecember
    }
    public enum ExpenseCategory
    {
        Uncategorized,
        Food,
        HouseStuff,
        Transport,
        Freetime,
        Kids,
        Streaming,
        SaaS
    }
    public class Expense
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public ExpenseCategory ExpenseCategory { get; set; } = ExpenseCategory.Uncategorized;
        public ExpenseRecurrence ExpenseRecurrence { get; set; } = ExpenseRecurrence.OneTime;
    }
}
