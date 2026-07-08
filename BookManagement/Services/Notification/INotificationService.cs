using System;

namespace BookManagement.Services.Notification
{
    public interface INotificationService
    {
        void ShowNotification(string message, string status = "Success");
        event Action<string, string> NotificationRequested;
    }
}
