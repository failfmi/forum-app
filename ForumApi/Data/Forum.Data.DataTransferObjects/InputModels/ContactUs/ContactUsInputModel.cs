using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.ContactUs
{
    public class ContactUsInputModel
    {
        [Required, EmailAddress, MinLength(4, ErrorMessage = "Email must be at least 4 symbols"), MaxLength(50, ErrorMessage = "Email must be maximum 50 symbols.")]
        public string Email { get; set; }

        [Required, MinLength(5, ErrorMessage = "Subject must be at least 5 symbols."), MaxLength(100, ErrorMessage = "Subject must be maximum 50 symbols.")]
        public string Subject { get; set; }

        [Required, MinLength(20, ErrorMessage = "Description must be at least 20 symbols."), MaxLength(1000, ErrorMessage = "Description must be maximum 1 000 symbols.")]
        public string Description { get; set; }
    }
}
