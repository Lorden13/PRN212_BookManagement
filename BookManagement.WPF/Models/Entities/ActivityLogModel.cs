namespace BookManagement.Models.Entities
{
    public class ActivityLogModel
    {
        public string Message { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Purchase, Submit, Register
    }
}
