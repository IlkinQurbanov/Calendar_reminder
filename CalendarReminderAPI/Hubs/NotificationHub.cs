using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CalendarReminderApi.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendTestNotification(string message)
        {
            await Clients.Caller.SendAsync("ReceiveNotification", new
            {
                Id = 0,
                Title = "Test Notification",
                Description = message
            });

            _logger.LogInformation($"Test notification sent to {Context.ConnectionId}: {message}");
        }
    }
}