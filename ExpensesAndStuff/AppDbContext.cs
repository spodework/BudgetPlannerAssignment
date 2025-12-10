using ExpensesAndStuff.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensesAndStuff
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Absence> Absences { get; set; }

        public string DbPath { get; }

        // Parameterless constructor for EF Core tools
        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "budgetplanner.db");
        }

        // Constructor for dependency injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "budgetplanner.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite($"Data Source={DbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Income>()
                .Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Expense>()
                 .Property(e => e.Date)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Absence>()
                 .Property(e => e.Date)
                 .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}