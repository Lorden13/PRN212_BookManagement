using BookManagement.WPF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Services.RoleService
{
    public class RoleService
    {
        private readonly ProjectPrnContext _context;
        public RoleService()
        {
            _context = new ProjectPrnContext();

        }
        public async Task<Role> GetByNameAsync(string name)
        {
            return await _context.Roles.Where(q => q.RoleName == name).FirstOrDefaultAsync();
        }
    }
}
