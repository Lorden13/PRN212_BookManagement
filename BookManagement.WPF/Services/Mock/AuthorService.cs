using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
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
        private static readonly Dictionary<int, string> _idMap = new Dictionary<int, string>();
        
        // In-memory caches for fields that don't exist in SQL Server schema
        private static readonly Dictionary<string, string> _bioCache = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _joinedDateCache = new Dictionary<string, string>();

        public AuthorService()
        {
            _dbContext = new ProjectPrnContext();
        }

        private static int GetIntId(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return 0;
            unchecked
            {
                int hash = (int)2166136261;
                foreach (char c in guid)
                {
                    hash = (hash ^ c) * 16777619;
                }
                int finalId = Math.Abs(hash);
                _idMap[finalId] = guid;
                return finalId;
            }
        }

        public IEnumerable<AuthorModel> GetAllAuthors()
        {
            var accounts = _dbContext.Accounts.Include(a => a.Role).Where(a => a.Role.RoleName == "Author").ToList();
            return accounts.Select(MapToModel);
        }

        public AuthorModel GetAuthorById(int id)
        {
            if (!_idMap.TryGetValue(id, out string? guid))
            {
                // Fallback to match by hashing all authors if the ID isn't in map yet
                var accounts = _dbContext.Accounts.Include(a => a.Role).Where(a => a.Role.RoleName == "Author").ToList();
                foreach (var acc in accounts)
                {
                    GetIntId(acc.AccountId);
                }
                if (!_idMap.TryGetValue(id, out guid))
                {
                    return null;
                }
            }

            var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == guid);
            return account == null ? null : MapToModel(account);
        }

        public AuthorModel GetAuthorByAccountId(string accountId)
        {
            var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == accountId);
            return account == null ? null : MapToModel(account);
        }

        private string? ResolveGuid(int id)
        {
            if (_idMap.TryGetValue(id, out string? guid))
            {
                return guid;
            }

            // Fallback: re-hash all author accounts to populate the map, then retry
            var accounts = _dbContext.Accounts.Include(a => a.Role).Where(a => a.Role.RoleName == "Author").ToList();
            foreach (var acc in accounts)
            {
                GetIntId(acc.AccountId);
            }

            return _idMap.TryGetValue(id, out guid) ? guid : null;
        }

        public bool UpdateAuthor(AuthorModel author)
        {
            try
            {
                string? guid = ResolveGuid(author.Id);
                if (guid == null) return false;

                var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == guid);
                if (account == null) return false;

                account.FullName = author.Name;
                account.Email = author.Email;
                account.Phone = author.Phone;
                account.Address = author.Address;
                account.IsActive = author.Status == "Active";

                _dbContext.Accounts.Update(account);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetAuthorActive(int id, bool isActive)
        {
            string? guid = ResolveGuid(id);
            if (guid == null) return;

            var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == guid);
            if (account != null)
            {
                account.IsActive = isActive;
                _dbContext.Accounts.Update(account);
                _dbContext.SaveChanges();
            }
        }

        public void UpdateProfile(AuthorModel author)
        {
            string? guid = null;
            if (BookManagement.Services.Utils.UserSession.CurrentUser != null && GetIntId(BookManagement.Services.Utils.UserSession.CurrentUser.AccountId) == author.Id)
            {
                guid = BookManagement.Services.Utils.UserSession.CurrentUser.AccountId;
            }
            else
            {
                _idMap.TryGetValue(author.Id, out guid);
            }

            if (guid == null) return;

            var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == guid);
            if (account != null)
            {
                account.FullName = author.Name;
                account.Email = author.Email;
                account.Phone = author.Phone;
                account.Address = author.Address;
                
                // Update in-memory cache for bio
                _bioCache[guid] = author.Bio;

                // Also update local session cache to stay synchronized in sidebar/header immediately
                if (BookManagement.Services.Utils.UserSession.CurrentUser != null && BookManagement.Services.Utils.UserSession.CurrentUser.AccountId == guid)
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
            string guid = account.AccountId;
            if (!_bioCache.TryGetValue(guid, out string? bio))
            {
                bio = "C# developer and technical author."; // Default bio
                _bioCache[guid] = bio;
            }
            if (!_joinedDateCache.TryGetValue(guid, out string? joinedDate))
            {
                joinedDate = DateTime.Now.ToString("yyyy-MM-dd"); // Default joined date
                _joinedDateCache[guid] = joinedDate;
            }

            return new AuthorModel
            {
                Id = GetIntId(guid),
                Name = account.FullName,
                Email = account.Email,
                Bio = bio,
                JoinedDate = joinedDate,
                Phone = account.Phone ?? "",
                Address = account.Address ?? "",
                AvatarPath = "/Assets/Avatars/author.png",
                Status = account.IsActive ? "Active" : "Inactive"
            };
        }
    }
}
