using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.User
{
    public class RegisterUserInputModel
    {
        private const int UsernameMinLength = 4;
        private const int PasswordMinLength = 6;

        [Required]
        [MinLength(UsernameMinLength, ErrorMessage = "Username must be at least 4 symbols.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid e-mail.")]
        public string Email { get; set; }

        [Required]
        [MinLength(PasswordMinLength, ErrorMessage = "Password must be at least 6 symbols.")]
        public string Password { get; set; }
    }
}
