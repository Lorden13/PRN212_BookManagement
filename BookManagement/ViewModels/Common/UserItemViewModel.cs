namespace BookManagement.ViewModels.Common
{
    public class UserItemViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Reader or Author
        public string Status { get; set; } = "Active"; // Active or Inactive
        public string JoinedDate { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }
}
