using ExpensesAndStuff.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesAndStuff.Interfaces
{
    public class IncomeService
    {
        private readonly AppDbContext _context;

        public IncomeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Income>> GetIncomesAsync()
        {
            return await _context.Incomes.ToListAsync();
        }

        public async Task<Income> FindIncomeAsync(int id)
        {
            return await _context.Incomes.FindAsync(id);
        }

        public async Task<Income> FindIncomeByCategoryAsync(IncomeCategory category)
        {
            return await _context.Incomes
                .FirstOrDefaultAsync(i => i.IncomeCategory == category);
        }

        public async Task<List<Income>> FindMultipleIncomesByCategoryAsync(IncomeCategory category)
        {
            return await _context.Incomes
                .Where(i => i.IncomeCategory == category)
                .ToListAsync();
        }


        // Asynchronous method for deleting an expense
        public async Task<bool> DeleteIncomeAsync(int expenseId)
        {
            var existingExpense = await _context.Incomes
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            if (existingExpense == null)
            {
                return false; // Expense not found
            }

            _context.Incomes.Remove(existingExpense);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }

        public async Task AddAsync(Income income)
        {
            await _context.Incomes.AddAsync(income);
            await _context.SaveChangesAsync(); // for saving ID
        }

        public async Task<bool> UpdateAsync(Income income)
        {
            var existingIncome = await _context.Incomes
                .FirstOrDefaultAsync(e => e.Id == income.Id);

            if (existingIncome == null)
                return false;

            // Update properties            
            existingIncome.Amount = income.Amount;
            existingIncome.IncomeCategory = income.IncomeCategory;
            existingIncome.IncomeRecurrence = income.IncomeRecurrence;


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
