
namespace BookManagement.Services.Mock
{
    public class MockUserService : IUserService
    {
        public UserProfileModel AuthenticateDemo(string role)
        {
            switch (role.ToLower())
            {
                case "reader":
                    return new UserProfileModel
                    {
                        Name = "John Smith",
                        Email = "reader@gmail.com",
                        Role = "Reader",
                        JoinedDate = "2026-05-10"
                    };
                case "author":
                    return new UserProfileModel
                    {
                        Name = "Alice Johnson",
                        Email = "author@email.com",
                        Role = "Author",
                        JoinedDate = "2026-02-15"
                    };
                case "admin":
                    return new UserProfileModel
                    {
                        Name = "System Administrator",
                        Email = "admin@gmail.com",
                        Role = "Admin",
                        JoinedDate = "2026-01-01"
                    };
                default:
                    return null;
            }
        }
    }
}
