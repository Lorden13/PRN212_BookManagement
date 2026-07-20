using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BookManagement.WPF.Services.Transactions
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
                    _prnContext.Accounts.Update(account);
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
    }
}
