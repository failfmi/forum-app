using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.ContactUs
{
    public class ContactUsInputModel
    {
        private const int SubjectMinLength = 5;
        private const int SubjectMaxLength = 100;
        private const string SubjectMessage = "Subject must be between 5 symbols and 100 symbols";

        private const int DescriptionMinLength = 10;
        private const int DescriptionMaxLength = 2000;
        private const string DescriptionMessage = "Description must be between 10 symbols and 2000 symbols";

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(SubjectMaxLength, MinimumLength = SubjectMinLength, ErrorMessage = SubjectMessage)]
        public string Subject { get; set; }

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = DescriptionMessage)]
        public string Description { get; set; }
    }
}
