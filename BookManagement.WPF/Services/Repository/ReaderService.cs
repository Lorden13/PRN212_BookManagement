using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookManagement.Services.Repository
{
    public class ReaderService : IReaderService
    {
        private readonly ProjectPrnContext _dbContext;

        public ReaderService()
        {
            _dbContext = new ProjectPrnContext();
        }

        public IEnumerable<ReaderModel> GetAllReaders()
        {
            var accounts = _dbContext.Accounts
                .Include(a => a.Role)
                .Where(a => a.Role.RoleName == "Reader")
                .ToList();
            return accounts.Select(MapToModel).ToList();
        }

        public ReaderModel GetReaderById(string id)
        {
            var account = _dbContext.Accounts
                .Include(a => a.Role)
                .FirstOrDefault(a => a.AccountId == id);
            return account == null ? null! : MapToModel(account);
        }

        public void UpdateProfile(ReaderModel reader)
        {
            if (string.IsNullOrEmpty(reader.Id)) return;

            var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == reader.Id);
            if (account != null)
            {
                account.FullName = reader.Name;
                account.Email = reader.Email;
                account.Phone = reader.Phone;
                account.Address = reader.Address;

                if (BookManagement.Services.Utils.UserSession.CurrentUser != null && BookManagement.Services.Utils.UserSession.CurrentUser.AccountId == reader.Id)
                {
                    BookManagement.Services.Utils.UserSession.CurrentUser.FullName = reader.Name;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Email = reader.Email;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Phone = reader.Phone;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Address = reader.Address;
                }

                _dbContext.Accounts.Update(account);
                _dbContext.SaveChanges();
            }
        }

        private ReaderModel MapToModel(Account account)
        {
            return new ReaderModel
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
