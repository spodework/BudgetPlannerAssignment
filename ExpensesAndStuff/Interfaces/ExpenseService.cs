using ExpensesAndStuff.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesAndStuff.Interfaces
{
    public class ExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetExpensesAsync()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task<Expense> GetExpenseAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }



        // Asynchronous method for deleting an expense
        public async Task<bool> DeleteExpenseAsync(int expenseId)
        {
            var existingExpense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            if (existingExpense == null)
            {
                return false; // Expense not found
            }

            _context.Expenses.Remove(existingExpense);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }

        public async Task AddAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync(); // for saving ID
        }

        public async Task<bool> UpdateAsync(Expense expense)
        {
            var existingExpense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expense.Id);

            if (existingExpense == null)
                return false;

            // Update properties            
            existingExpense.Amount = expense.Amount;
            existingExpense.ExpenseCategory = expense.ExpenseCategory;
            existingExpense.ExpenseRecurrence= expense.ExpenseRecurrence;


            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
