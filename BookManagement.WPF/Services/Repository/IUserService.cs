
using BookManagement.Models.Entities;
using BookManagement.WPF.Entities;

namespace BookManagement.Services.Repository
{
    public interface IUserService
    {
        UserProfileModel AuthenticateDemo(string role);
        Account? Login(string email, string password);
    }
}
