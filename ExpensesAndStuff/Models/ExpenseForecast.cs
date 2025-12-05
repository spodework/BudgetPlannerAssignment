namespace ExpensesAndStuff.Models
{
    public class ExpenseForecast
    {
        // add together monthly expenses and also if specifically matches next month
        public static decimal ForecastNextMonth(List<Expense> historicalExpenses, int thisMonth)
        {
            if (historicalExpenses == null || historicalExpenses.Count == 0)
                return 0;

            int nextMonth = (thisMonth % 12) + 1;

            decimal forecastedAmount = 0;
            foreach (var expense in historicalExpenses)
            {
                forecastedAmount += GetExpenseForNextMonth(expense, nextMonth);
            }

            return forecastedAmount;
        }
        
        // todo: something else this is jank
        public static decimal GetExpenseForNextMonth(Expense expense, int nextMonth)
        {
            switch (expense.ExpenseRecurrence)
            {
                case ExpenseRecurrence.OneTime:
                    return 0; // skip

                case ExpenseRecurrence.Monthly:
                    return expense.Amount; // always include

                case ExpenseRecurrence.YearlyJanuary:
                    return nextMonth == 1 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyFebruary:
                    return nextMonth == 2 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyMarch:
                    return nextMonth == 3 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyApril:
                    return nextMonth == 4 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyMay:
                    return nextMonth == 5 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyJune:
                    return nextMonth == 6 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyJuly:
                    return nextMonth == 7 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyAugust:
                    return nextMonth == 8 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlySeptember:
                    return nextMonth == 9 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyOctober:
                    return nextMonth == 10 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyNovember:
                    return nextMonth == 11 ? expense.Amount : 0;
                case ExpenseRecurrence.YearlyDecember:
                    return nextMonth == 12 ? expense.Amount : 0;

                default:
                    return 0;
            }
        }


    }

}
