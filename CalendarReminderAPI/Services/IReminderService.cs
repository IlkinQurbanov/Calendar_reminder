using CalendarReminderAPI.Models;

namespace CalendarReminderAPI.Services
{
    public interface IReminderService
    {
        IQueryable<Reminder> GetQueryableReminders();
        Task<IEnumerable<Reminder>> FilterRemindersAsync(
       string titleFilter = null,
       string descriptionFilter = null,
       DateTime? createdAfter = null,
       DateTime? createdBefore = null);

        Task<IEnumerable<Reminder>> GetRemindersAsync();
        Task<Reminder> GetReminderByIdAsync(int id);
        Task<Reminder> CreateReminderAsync(Reminder reminder);
        Task<Reminder> UpdateReminderAsync(int id, Reminder reminder);
        Task<bool> DeleteReminderAsync(int id);
        Task<byte[]> ExportToCsvAsync();
        Task<bool> IsDuplicateReminderAsync(DateTime reminderDateTime); // New method
    }
}