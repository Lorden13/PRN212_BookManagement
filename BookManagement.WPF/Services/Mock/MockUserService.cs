
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
                        JoinedDate = "2026-05-10",
                        PhoneNumber = "0987654321",
                        Address = "123 Reader Lane, New York"
                    };
                case "author":
                    return new UserProfileModel
                    {
                        Name = "Alice Johnson",
                        Email = "author@email.com",
                        Role = "Author",
                        JoinedDate = "2026-02-15",
                        PhoneNumber = "0912345678",
                        Address = "456 Author Blvd, San Francisco"
                    };
                case "admin":
                    return new UserProfileModel
                    {
                        Name = "System Administrator",
                        Email = "admin@gmail.com",
                        Role = "Admin",
                        JoinedDate = "2026-01-01",
                        PhoneNumber = "0900000000",
                        Address = "789 System Center, Washington DC"
                    };
                default:
                    return null;
            }
        }
    }
}
