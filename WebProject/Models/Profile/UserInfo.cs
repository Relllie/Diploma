using System;
using System.Collections.Generic;
using WebProject.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace WebProject.Models
{
    public class UserInfo
    {
        public int UserId { get; set; }
        public string SurName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? StudentIDNumber { get; set; }
        public bool ConfirmedStudentId { get; set; }
        public string Email { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
        public string? AvatarPath { get; set; }
        public IEnumerable<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
