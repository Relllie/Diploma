using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
        }

        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
