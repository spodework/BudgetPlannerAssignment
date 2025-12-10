using ExpensesAndStuff.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesAndStuff.Interfaces
{
    public class AbsenceService
    {
        private readonly AppDbContext _context;

        public AbsenceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Absence>> GetAbsencesAsync()
        {
            return await _context.Absences.ToListAsync();
        }

        public async Task<List<Absence>> GetThisMonthAbsencesAsync()
        {
            var now = DateTime.Now;

            var currentMonthAbsences = await _context.Absences
                .Where(a => a.Date.Month == now.Month && a.Date.Year == now.Year)
                .ToListAsync();

            return currentMonthAbsences;
        }

        public async Task<Absence> GetAbsenceAsync(int id)
        {
            return await _context.Absences.FindAsync(id);
        }

        public async Task<bool> DeleteAbsenceAsync(int absenceId)
        {
            var existingAbsence = await _context.Absences
                .FirstOrDefaultAsync(e => e.Id == absenceId);

            if (existingAbsence == null)
            {
                return false; // Absence not found
            }

            _context.Absences.Remove(existingAbsence);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }

        public async Task AddAsync(Absence absence)
        {
            await _context.Absences.AddAsync(absence);
            await _context.SaveChangesAsync(); // for saving ID
        }

        public async Task<bool> UpdateAsync(Absence absence)
        {
            var existingAbsence = await _context.Absences
                .FirstOrDefaultAsync(e => e.Id == absence.Id);

            if (existingAbsence == null)
                return false;

            // Update properties            
            existingAbsence.Date = absence.Date;
            existingAbsence.Type = absence.Type;

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
