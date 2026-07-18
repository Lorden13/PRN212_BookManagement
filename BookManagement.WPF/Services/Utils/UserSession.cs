using BookManagement.WPF.Entities;

namespace BookManagement.Services.Utils
{
    public static class UserSession
    {
        public static CurrentUserModel? CurrentUser { get; set; }
    }

    public class CurrentUserModel
    {
        public string AccountId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }

        public string Role { get; set; } = "";
    }
}
