namespace CalendarReminderAPI.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ReminderDateTime { get; set; }
        public bool IsNotified { get; set; }
    }
}
