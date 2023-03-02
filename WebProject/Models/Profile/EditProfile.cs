using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class EditProfile
    {
        public string SurName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? StudentIDNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}
