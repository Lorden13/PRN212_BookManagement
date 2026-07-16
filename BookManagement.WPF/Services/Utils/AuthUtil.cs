using BookManagement.Models.Auth;
using BookManagement.SQLite;
using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Services.Utils
{
    public class AuthUtil
    {
        //get token tu sqlite
        //xuong db check token trong bang token 
        // neu hop le tra ve accID, role, username

        private readonly ProjectPrnContext _context;
        private readonly UserSecretContext _sql;

        public AuthUtil()
        {
            _context = new ProjectPrnContext();
            _sql = new UserSecretContext();
        }

        public async Task<CheckTokenResult> CheckTokenAsync()
        {
            try
            {
                await _sql.Database.EnsureCreatedAsync();
                SavedToken token = await _sql.SavedTokens.FirstOrDefaultAsync();
                if(token == null)
                {
                    return null;
                }

                Token tokenDb = await _context.Tokens.Where(q => q.TokenValue == token.TokenValue && !q.IsRevoked && q.ExpiredDate > DateTime.Now).FirstOrDefaultAsync();
                if (tokenDb != null)
                {
                    Account result = await _context.Accounts.Include("Role").Where(q => q.AccountId == tokenDb.AccountId).FirstOrDefaultAsync();
                    if (result != null)
                    {
                        return new CheckTokenResult()
                        {
                            AccId = tokenDb.AccountId,
                            Email = result.Email,
                            RoleName = result.Role.RoleName
                        };
                    }
                    else return null;

                }
                else
                {
                    return null;
                }

                
            }catch(Exception ex)
            {
                return null;
            }
        }

        
        
    }
}
