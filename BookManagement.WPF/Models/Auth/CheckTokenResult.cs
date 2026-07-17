using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement.Models.Auth
{
    public class CheckTokenResult
    {
        public string AccId { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
    }
}
