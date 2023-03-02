using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class RegisterModel
    {

        [Required(ErrorMessage = "Не указана фамилия")]
        public string SurName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? StudentIDNumber { get; set; }
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}