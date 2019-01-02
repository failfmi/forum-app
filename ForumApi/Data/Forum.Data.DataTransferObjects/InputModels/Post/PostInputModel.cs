using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Post
{
    public class PostInputModel
    {
        [Required]
        public int CategoryId { get; set; }

        [Required, MinLength(6, ErrorMessage = "Post title must be at least 6 symbols."), MaxLength(50, ErrorMessage = "Post title must be maximum 50 symbols.")]
        public string Title { get; set; }

        [Required, MinLength(10, ErrorMessage = "Post body must be at least 10 symbols."), MaxLength(2000, ErrorMessage = "Post body must be maximum 1000 symbols.")]
        public string Body { get; set; }
    }
}
