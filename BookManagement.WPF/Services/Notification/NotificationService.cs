using System;

namespace BookManagement.Services.Notification
{
    public class NotificationService : INotificationService
    {
        public event Action<string, string>? NotificationRequested;

        public void ShowNotification(string message, string status = "Success")
        {
            NotificationRequested?.Invoke(message, status);
        }
    }
}
