using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Comment
{
    public class CommentInputEditModel
    {
        [Required]
        public int Id { get; set; }

        [Required, MinLength(6, ErrorMessage = "Comment Edit Text must be at least 6 symbols."), MaxLength(200, ErrorMessage = "Comment Text must be maximum 200 symbols.")]
        public string Text { get; set; }
    }
}
