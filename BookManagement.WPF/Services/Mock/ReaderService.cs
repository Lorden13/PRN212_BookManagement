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
        private static readonly Dictionary<int, string> _idMap = new Dictionary<int, string>();
        private static readonly Dictionary<string, string> _joinedDateCache = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _avatarCache = new Dictionary<string, string>();

        public ReaderService()
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

        public IEnumerable<ReaderModel> GetAllReaders()
        {
            var accounts = _dbContext.Accounts.Include(a => a.Role).Where(a => a.Role.RoleName == "Reader").ToList();
            return accounts.Select(MapToModel);
        }

        public ReaderModel GetReaderById(int id)
        {
            if (!_idMap.TryGetValue(id, out string? guid))
            {
                var accounts = _dbContext.Accounts.Include(a => a.Role).Where(a => a.Role.RoleName == "Reader").ToList();
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

        public ReaderModel GetReaderByAccountId(string accountId)
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

            // Fallback: re-hash all reader accounts to populate the map, then retry
            var accounts = _dbContext.Accounts.Include(a => a.Role).Where(a => a.Role.RoleName == "Reader").ToList();
            foreach (var acc in accounts)
            {
                GetIntId(acc.AccountId);
            }

            return _idMap.TryGetValue(id, out guid) ? guid : null;
        }

        public bool UpdateReader(ReaderModel reader)
        {
            try
            {
                string? guid = ResolveGuid(reader.Id);
                if (guid == null) return false;

                var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == guid);
                if (account == null) return false;

                account.FullName = reader.Name;
                account.Email = reader.Email;
                account.Phone = reader.Phone;
                account.Address = reader.Address;
                account.IsActive = reader.Status == "Active";

                _dbContext.Accounts.Update(account);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetReaderActive(int id, bool isActive)
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

        public void UpdateProfile(ReaderModel reader)
        {
            string? guid = null;
            if (BookManagement.Services.Utils.UserSession.CurrentUser != null && GetIntId(BookManagement.Services.Utils.UserSession.CurrentUser.AccountId) == reader.Id)
            {
                guid = BookManagement.Services.Utils.UserSession.CurrentUser.AccountId;
            }
            else
            {
                _idMap.TryGetValue(reader.Id, out guid);
            }

            if (guid == null) return;

            var account = _dbContext.Accounts.FirstOrDefault(a => a.AccountId == guid);
            if (account != null)
            {
                account.FullName = reader.Name;
                account.Email = reader.Email;
                account.Phone = reader.Phone;
                
                _avatarCache[guid] = reader.AvatarPath;

                if (BookManagement.Services.Utils.UserSession.CurrentUser != null && BookManagement.Services.Utils.UserSession.CurrentUser.AccountId == guid)
                {
                    BookManagement.Services.Utils.UserSession.CurrentUser.FullName = reader.Name;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Email = reader.Email;
                    BookManagement.Services.Utils.UserSession.CurrentUser.Phone = reader.Phone;
                }

                _dbContext.Accounts.Update(account);
                _dbContext.SaveChanges();
            }
        }

        private ReaderModel MapToModel(Account account)
        {
            string guid = account.AccountId;
            if (!_joinedDateCache.TryGetValue(guid, out string? joinedDate))
            {
                joinedDate = DateTime.Now.ToString("yyyy-MM-dd");
                _joinedDateCache[guid] = joinedDate;
            }
            if (!_avatarCache.TryGetValue(guid, out string? avatar))
            {
                avatar = "IconUser";
                _avatarCache[guid] = avatar;
            }

            return new ReaderModel
            {
                Id = GetIntId(guid),
                Name = account.FullName,
                Email = account.Email,
                Phone = account.Phone ?? "",
                Address = account.Address ?? "",
                AvatarPath = avatar,
                JoinedDate = joinedDate,
                Status = account.IsActive ? "Active" : "Inactive"
            };
        }
    }
}
