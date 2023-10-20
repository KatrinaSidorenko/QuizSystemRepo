using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.UserViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email Address is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        //[DataType(DataType.Password)]
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
