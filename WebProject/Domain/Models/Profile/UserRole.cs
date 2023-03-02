using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Domain.Models
{
    public class UserRole
    {
        public int UserRoleID { get; set; }
        public int UserID { get; set; }
        public int? RoleID { get; set; }
        public Role Role { get; set; } = new Role();
    }
}
