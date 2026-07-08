namespace BookManagement.Models.Entities
{
    public class NotificationModel
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
