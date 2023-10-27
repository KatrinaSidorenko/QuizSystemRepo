using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.UserViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email Address is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
