using CalendarReminderAPI.Models;

namespace CalendarReminderAPI.Services
{
    public interface IReminderService
    {
        Task<IEnumerable<Reminder>> GetRemindersAsync();
        Task<Reminder> GetReminderByIdAsync(int id);
        Task<Reminder> CreateReminderAsync(Reminder reminder);
        Task<Reminder> UpdateReminderAsync(int id, Reminder reminder);
        Task<bool> DeleteReminderAsync(int id);
        Task<byte[]> ExportToCsvAsync();
        Task<bool> IsDuplicateReminderAsync(DateTime reminderDateTime); // New method
    }
}