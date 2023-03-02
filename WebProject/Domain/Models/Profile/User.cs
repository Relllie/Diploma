using System;
using System.Security.Cryptography;

namespace WebProject.Domain.Models
{
    public class User
    {
        public User()
        {
        }

        public int UserId { get; set; }
        public string SurName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? StudentIDNumber { get; set; }
        public bool ConfirmedStudentId { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordSalt { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastSiteAccessDate { get; set; }
    }
}
