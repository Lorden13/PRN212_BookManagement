using BookManagement.Models.Entities;
using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookManagement.Services.Repository
{
    public class AuthorService : IAuthorService
    {
        private readonly ProjectPrnContext _dbContext;

        public AuthorService()
        {
            _dbContext = new ProjectPrnContext();
        }

        public IEnumerable<AuthorModel> GetAllAuthors()
        {
            var accounts = _dbContext.Accounts
                .Include(a => a.Role)
                .Where(a => a.Role.RoleName == "Author")
                .ToList();
            return accounts.Select(MapToModel).ToList();
        }

        public AuthorModel GetAuthorById(string id)
        {
            var account = _dbContext.Accounts
                .Include(a => a.Role)
                .FirstOrDefault(a => a.AccountId == id);
            return account == null ? null! : MapToModel(account);
        }

        public AuthorModel GetAuthorByAccountId(string accountId)
        {
            return GetAuthorById(accountId);
        }

        public void UpdateProfile(AuthorModel author)
        {
            if (string.IsNullOrEmpty(author.Id)) return;

            var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == author.Id);
            if (account != null)
            {
                account.FullName = author.Name;
                account.Email = author.Email;
                account.Phone = author.Phone;
                account.Address = author.Address;

                if (BookManagement.Services.Utils.UserSession.CurrentUser != null && BookManagement.Services.Utils.UserSession.CurrentUser.AccountId == author.Id)
                {
                    BookManagement.Services.Utils.UserSession.CurrentUser.FullName = author.Name;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Email = author.Email;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Phone = author.Phone;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Address = author.Address;
                }

                _dbContext.Accounts.Update(account);
                _dbContext.SaveChanges();
            }
        }

            private AuthorModel MapToModel(Account account)
            {
                return new AuthorModel
                {
                    Id = account.AccountId,
                    Name = account.FullName,
                    Email = account.Email,
                    Phone = account.Phone ?? "",
                    Address = account.Address ?? "",
                    Status = account.IsActive ? "Active" : "Inactive"
                };
            }
    }
}
