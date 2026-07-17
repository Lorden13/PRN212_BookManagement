using BookManagement.WPF.Entities;

namespace BookManagement.Services.Utils
{
    public static class UserSession
    {
        public static Account? CurrentUser { get; set; }
    }
}
