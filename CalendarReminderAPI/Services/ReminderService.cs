﻿using CalendarReminderAPI.Data;
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
    }
}