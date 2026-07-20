using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IAuthorService
    {
        IEnumerable<AuthorModel> GetAllAuthors();
        AuthorModel GetAuthorById(string id);
        void UpdateProfile(AuthorModel author);
        AuthorModel GetAuthorByAccountId(string accountId);
    }
}
