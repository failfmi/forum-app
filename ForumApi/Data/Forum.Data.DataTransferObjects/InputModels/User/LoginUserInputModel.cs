using System.ComponentModel.DataAnnotations;

namespace Forum.Data.DataTransferObjects.InputModels.User
{
    public class LoginUserInputModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
