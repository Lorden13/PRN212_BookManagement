using BookManagement.Entities;
using BookManagement.Services.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookManagement.Services.ReaderService
{
    public class ReaderService
    {
        private readonly ProjectPrnContext _prnContext;
        private readonly string PRIVATEKEY = "9jS7vSNXDmhVssdcgjRunoyKQpeOSndd3DzK8sBetn2yzPqdYAn9R0+*^R&$S80kC4bUuKveVJVt";

        public ReaderService()
        {
            _prnContext = new ProjectPrnContext();
        }

        private async Task<bool> IsDuplicateEmail(string email)
        {
            Account accountDb = await _prnContext.Accounts.Where(q => q.Email.ToLower().Equals(email.ToLower())).FirstOrDefaultAsync();
            if (accountDb == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> CreateReaderAsync(Account account)
        {
            try
            {
                if (!await IsDuplicateEmail(account.Email))
                {
                    string hashedPassword = HashBuilder.ComputeSha256Hash(account.Password + PRIVATEKEY);
                    string accId = Guid.NewGuid().ToString();
                    string roleId = (await _prnContext.Roles.Where(q => q.RoleName.Equals("Reader")).FirstOrDefaultAsync()).RoleId; //Lấy ra được mới bốc role id
                    Account newAccount = new Account()
                    {
                        AccountId = accId,
                        RoleId = roleId,
                        Email = account.Email,
                        Password = hashedPassword,
                        FullName = account.FullName,
                        Address = account.Address,
                        Phone = account.Phone,
                        IsActive = true
                    };  
                    
                    await _prnContext.Accounts.AddAsync(newAccount);

                    Reader newReader = new Reader()
                    {
                        ReaderId = accId
                    };

                    await _prnContext.Readers.AddAsync(newReader);

                  
                    await _prnContext.SaveChangesAsync();
                    return true;
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
