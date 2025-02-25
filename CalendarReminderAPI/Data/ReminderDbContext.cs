using CalendarReminderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarReminderAPI.Data
{
    public class ReminderDbContext : DbContext
    {
        public ReminderDbContext(DbContextOptions<ReminderDbContext> options) : base(options)
        {
        }

        public DbSet<Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reminder>()
                .Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Reminder>()
                .Property(r => r.Description)
                .HasMaxLength(500);
        }
    }
}