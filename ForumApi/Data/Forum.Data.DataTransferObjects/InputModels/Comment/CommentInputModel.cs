using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Comment
{
    public class CommentInputModel
    {
        [Required, MinLength(6, ErrorMessage = "Comment Text must be at least 6 symbols")]
        public string Text { get; set; }

        [Required]
        public int PostId { get; set; }
    }
}
