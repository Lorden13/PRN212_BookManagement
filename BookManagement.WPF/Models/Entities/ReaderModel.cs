namespace BookManagement.Models.Entities
{
    public class ReaderModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JoinedDate { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }
}
