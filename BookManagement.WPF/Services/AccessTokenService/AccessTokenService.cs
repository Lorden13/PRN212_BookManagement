using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Services.AccessTokenService
{
    public class AccessTokenService
    {
        private readonly ProjectPrnContext _context;
        public AccessTokenService()
        {
            _context = new ProjectPrnContext();
        }

        public async Task<Account> CheckAccessToken(string accessToken)
        {
            Token aToken = await _context.Tokens.Include("Account").FirstOrDefaultAsync(q => q.TokenValue == accessToken && q.IsRevoked == false && q.ExpiredDate >  DateTime.Now);
            if (aToken == null) return null;
            else
            {
                Role role = await _context.Roles.FirstOrDefaultAsync(q => q.RoleId == aToken.Account.RoleId);
                aToken.Account.Role = role;
                return aToken.Account;
            }
        }

        public async Task<bool> RevokeTokenAsync(string accId)
        {
            Token aTokenDb = await _context.Tokens.FirstOrDefaultAsync(q => q.AccountId == accId);
            if(aTokenDb != null)
            {
                aTokenDb.IsRevoked = true;
                _context.Tokens.Update(aTokenDb);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> GenerateAccessTokenAsync(string accId)
        {
            //check access token exist
            Token aTokenDb = await _context.Tokens.FirstOrDefaultAsync(q => q.AccountId == accId);
            if(aTokenDb == null)
            {
                Token newTokenDb = new Token()
                {
                    AccountId = accId,
                    ExpiredDate = DateTime.Now.AddDays(14),
                    IsRevoked = false,
                    TokenId = Guid.NewGuid().ToString(),
                    TokenValue = Guid.NewGuid().ToString(),
                };
                await _context.Tokens.AddAsync(newTokenDb);
                await _context.SaveChangesAsync();
                return newTokenDb.TokenValue;
            }
            else
            {
                aTokenDb.TokenValue = Guid.NewGuid().ToString();
                aTokenDb.ExpiredDate = DateTime.Now.AddDays(14);
                aTokenDb.IsRevoked= false;
                _context.Tokens.Update(aTokenDb);
                await _context.SaveChangesAsync();
                return aTokenDb.TokenValue;
            }
        }
    }
}
