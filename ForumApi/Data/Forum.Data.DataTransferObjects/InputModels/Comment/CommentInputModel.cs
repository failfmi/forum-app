using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Comment
{
    public class CommentInputModel
    {
        private const int MinLength = 6;
        private const string MinLengthMessage = "Comment Text must be at least 6 symbols";

        private const int MaxLength = 200;
        private const string MaxLengthMessage = "Comment Text must be maximum 200 symbols.";

        [Required, MinLength(MinLength, ErrorMessage = MinLengthMessage), MaxLength(MaxLength, ErrorMessage = MaxLengthMessage)]
        public string Text { get; set; }

        [Required]
        public int PostId { get; set; }
    }
}
