﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Category
{
    public class CategoryInputModel
    {
        [Required, MinLength(3, ErrorMessage = "Category Name must be at least 3 symbols")]
        public string Name { get; set; }
    }
}