using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Category
{
    public class CategoryInputEditModel : CategoryInputModel
    {
        [Required]
        public int Id { get; set; }
    }
}
