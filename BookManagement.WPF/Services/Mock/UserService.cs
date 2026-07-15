using BookManagement.Models.Entities;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.Utils;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Services.Repository
{
    public class UserService : IUserService
    {
        private readonly ProjectPrnContext _prnContext;
        private readonly string PRIVATEKEY = "9jS7vSNXDmhVssdcgjRunoyKQpeOSndd3DzK8sBetn2yzPqdYAn9R0+*^R&$S80kC4bUuKveVJVt";

        public UserService()
        {
            _prnContext = new ProjectPrnContext();
        }

        public Account? Login(string email, string password)
        {
            try
            {
                string hashed = HashBuilder.ComputeSha256Hash(password + PRIVATEKEY);
                var account = _prnContext.Accounts
                    .Include(q => q.Role)
                    .FirstOrDefault(q => q.IsActive && q.Email == email.Trim());

                if (account == null)
                    return null;
                if (account.Password == hashed)
                    return account;

                // Upgrade legacy plaintext passwords created directly in SQL.
                if (account.Password == password)
                {
                    account.Password = hashed;
                    _prnContext.SaveChanges();
                    return account;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public UserProfileModel AuthenticateDemo(string role)
        {
            // Read from dynamic UserSession if available
            var currentUser = BookManagement.Services.Utils.UserSession.CurrentUser;
            if (currentUser != null)
            {
                return new UserProfileModel
                {
                    Name = currentUser.FullName,
                    Email = currentUser.Email,
                    Role = role,
                    JoinedDate = "2026-02-15", // placeholder as schema doesn't have it
                    PhoneNumber = currentUser.Phone ?? "",
                    Address = currentUser.Address ?? "",
                    AvatarPath = role == "Author" ? "/Assets/Avatars/author.png" : (role == "Reader" ? "/Assets/Avatars/reader.png" : "/Assets/Avatars/admin.png")
                };
            }

            // Fallback for design-time / no session
            return new UserProfileModel
            {
                Name = $"Demo {role}",
                Email = "demo@email.com",
                Role = role,
                JoinedDate = "2026-02-15",
                AvatarPath = "/Assets/Avatars/author.png",
                PhoneNumber = "",
                Address = ""
            };
        }
    }
}
