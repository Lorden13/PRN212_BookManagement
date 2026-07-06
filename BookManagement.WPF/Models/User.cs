using System;

namespace BookManagement.WPF.Models
{
    public enum UserRole
    {
        Admin,
        Author,
        Reader
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string AvatarInitials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName)) return "?";
                var parts = FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
                return parts[0][0].ToString().ToUpper();
            }
        }
    }
}
