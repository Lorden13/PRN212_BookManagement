using BookManagement.Entities;
using BookManagement.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Services.AccountService
{
    public class AccountService
    {
        private readonly ProjectPrnContext _prnContext;
        private readonly string PRIVATEKEY = "9jS7vSNXDmhVssdcgjRunoyKQpeOSndd3DzK8sBetn2yzPqdYAn9R0+*^R&$S80kC4bUuKveVJVt";

        public AccountService()
        {
            _prnContext = new ProjectPrnContext();
        }

        public async Task<Account> CheckLoginAsync(string email, string password)
        {
            return await _prnContext.Accounts.Include("Role").Where(q => q.Email.Equals(email) && q.Password.Equals(HashBuilder.ComputeSha256Hash(password + PRIVATEKEY))).FirstOrDefaultAsync();
        }

        public async Task<Account> GetAccountByIdAsync(string accountId)
        {
            return await _prnContext.Accounts.Where(q => q.AccountId.Equals(accountId)).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePassword(string accId, string oldpassword, string newPassword)
        {
            try
            {
                Account account = await _prnContext.Accounts.Where(q => q.AccountId == accId).FirstOrDefaultAsync();
                if(account != null)
                {
                    string hashedPass = HashBuilder.ComputeSha256Hash(oldpassword + PRIVATEKEY);
                    if(hashedPass == account.Password)
                    {
                        account.Password = HashBuilder.ComputeSha256Hash(newPassword + PRIVATEKEY);
                        _prnContext.Accounts.Update(account);
                        await _prnContext.SaveChangesAsync();
                        return true;
                    }
                    else return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) { return false; }
        }
    }
}
