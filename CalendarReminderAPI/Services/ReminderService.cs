using CalendarReminderAPI.Data;
using CalendarReminderAPI.Models;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CalendarReminderAPI.Services
{
    public class ReminderService : IReminderService
    {
        private readonly ReminderDbContext _context;

        public ReminderService(ReminderDbContext context)
        {
            _context = context;
        }

        public IQueryable<Reminder> GetQueryableReminders()
        {
            return _context.Reminders.AsQueryable();
        }
        public async Task<IEnumerable<Reminder>> FilterRemindersAsync(
        string title = null,
        string description = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null)
        {
            var query = _context.Reminders.AsQueryable();

            // Фильтр по заголовку (если указан)
            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(r => r.Title.Contains(title));

            // Фильтр по описанию (если указан)
            if (!string.IsNullOrWhiteSpace(description))
                query = query.Where(r => r.Description.Contains(description));

            // Фильтр по дате создания
            if (createdAfter.HasValue)
                query = query.Where(r => r.CreatedAt >= createdAfter.Value);

            if (createdBefore.HasValue)
                query = query.Where(r => r.CreatedAt <= createdBefore.Value);

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Reminder>> GetRemindersAsync()
        {
            return await _context.Reminders.ToListAsync();
        }

        public async Task<Reminder> GetReminderByIdAsync(int id)
        {
            return await _context.Reminders.FindAsync(id);
        }

        public async Task<Reminder> CreateReminderAsync(Reminder reminder)
        {
            // Check for duplicate reminders
            if (await IsDuplicateReminderAsync(reminder.ReminderDateTime))
            {
                throw new InvalidOperationException("A reminder already exists for the same hour on the same day.");
            }

            reminder.CreatedAt = DateTime.UtcNow;
            reminder.IsNotified = false;

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
            return reminder;
        }

        public async Task<Reminder> UpdateReminderAsync(int id, Reminder reminder)
        {
            var existingReminder = await _context.Reminders.FindAsync(id);
            if (existingReminder == null)
                return null;

            // Check for duplicate reminders (excluding the current reminder being updated)
            if (await IsDuplicateReminderAsync(reminder.ReminderDateTime) && existingReminder.Id != id)
            {
                throw new InvalidOperationException("A reminder already exists for the same hour on the same day.");
            }

            existingReminder.Title = reminder.Title;
            existingReminder.Description = reminder.Description;
            existingReminder.ReminderDateTime = reminder.ReminderDateTime;

            await _context.SaveChangesAsync();
            return existingReminder;
        }

        public async Task<bool> DeleteReminderAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
                return false;

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]> ExportToCsvAsync()
        {
            var reminders = await _context.Reminders.ToListAsync();
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(reminders);
            await writer.FlushAsync();
            return memoryStream.ToArray();
        }
        public async Task<bool> IsDuplicateReminderAsync(DateTime reminderDateTime)
        {
            var existingReminder = await _context.Reminders
                .FirstOrDefaultAsync(r =>
                    r.ReminderDateTime.Date == reminderDateTime.Date &&
                    r.ReminderDateTime.Hour == reminderDateTime.Hour);

            return existingReminder != null; 
        }

    }
}