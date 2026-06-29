using System.Collections.Generic;
using System.Linq;

namespace BookManagement.Services.Mock
{
    public class MockAuthorService : IAuthorService
    {
        private readonly List<AuthorModel> _authors = new List<AuthorModel>();

        public MockAuthorService()
        {
            _authors.Add(new AuthorModel { Id = 1, Name = "Alice Johnson", Email = "author@email.com", Bio = "C# developer and technical author.", JoinedDate = "2026-02-15", Phone = "0987 654 321", Address = "987 Pine St, Seattle, WA", Status = "Active" });
            _authors.Add(new AuthorModel { Id = 2, Name = "Robert C. Martin", Email = "unclebob@clean-coder.com", Bio = "Software consultant, author of Clean Code and Clean Architecture.", JoinedDate = "2026-01-10", Phone = "0876 543 210", Address = "543 Clean Ave, Chicago, IL", Status = "Active" });
            _authors.Add(new AuthorModel { Id = 3, Name = "Martin Fowler", Email = "martin@fowler.org", Bio = "Author of Refactoring, software designer and speaker at ThoughtWorks.", JoinedDate = "2026-01-20", Phone = "0765 432 109", Address = "21 Refactor Rd, Boston, MA", Status = "Active" });
            _authors.Add(new AuthorModel { Id = 4, Name = "J. K. Rowling", Email = "jkrowling@hogwarts.edu", Bio = "British author, philanthropist, film producer, and screenwriter.", JoinedDate = "2026-03-05", Phone = "0654 321 098", Address = "12 Hogwarts Way, London, UK", Status = "Active" });
            _authors.Add(new AuthorModel { Id = 5, Name = "Carl Sagan", Email = "carl.sagan@cosmos.org", Bio = "American astronomer, planetary scientist, cosmologist, and author.", JoinedDate = "2026-04-12", Phone = "0543 210 987", Address = "7 Cosmos Ln, Ithaca, NY", Status = "Inactive" });
        }

        public IEnumerable<AuthorModel> GetAllAuthors() => _authors;

        public AuthorModel GetAuthorById(int id) => _authors.FirstOrDefault(a => a.Id == id);

        public void UpdateProfile(AuthorModel author)
        {
            var existing = GetAuthorById(author.Id);
            if (existing != null)
            {
                existing.Name = author.Name;
                existing.Email = author.Email;
                existing.Bio = author.Bio;
            }
        }
    }
}
