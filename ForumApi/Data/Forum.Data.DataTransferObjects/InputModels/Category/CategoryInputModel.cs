using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Category
{
    public class CategoryInputModel
    {
        private const int NameMinLength = 3;
        private const int NameMaxLength = 20;
        private const string Message = "Category Name must be between 3 and 20 symbols";

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = Message)]
        public string Name { get; set; }
    }
}
