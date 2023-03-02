using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models
{
    public class ProfileViewModel
    {
        public string UserSurName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? StudentIDNumber { get; set; }
        public string AvatarPath { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastSiteAccessDate { get; set; }
    }
}
