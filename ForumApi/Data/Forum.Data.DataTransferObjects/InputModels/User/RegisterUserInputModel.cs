using System.ComponentModel.DataAnnotations;

namespace Forum.Data.DataTransferObjects.InputModels.User
{
    public class RegisterUserInputModel
    {
        private const int UsernameMinLength = 4;
        private const int PasswordMinLength = 6;
        private const string RegExWithout = "^[^@]*$";

        [Required]
        [MinLength(UsernameMinLength, ErrorMessage = "Username must be at least 4 symbols.")]
        [RegularExpression(RegExWithout, ErrorMessage = "Username name must not have symbol \"@\" in itself.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid e-mail.")]
        public string Email { get; set; }

        [Required]
        [MinLength(PasswordMinLength, ErrorMessage = "Password must be at least 6 symbols.")]
        public string Password { get; set; }
    }
}
