using ExpensesAndStuff.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesAndStuff.Data
{
    public class UserService
    {

        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (existingUser == null)
            {
                return false; // User not found
            }

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == user.Id);

            if (existingUser == null)
                return false;

            // Update properties            
            existingUser.YearlyWorkHours = user.YearlyWorkHours;
            existingUser.YearlySalary = user.YearlySalary;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(); // for saving ID

            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
