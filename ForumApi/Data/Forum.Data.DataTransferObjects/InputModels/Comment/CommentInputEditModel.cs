using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Comment
{
    public class CommentInputEditModel
    {
        private const int CommentMinLength = 6;
        private const int CommentMaxLength = 200;
        private const string Message = "Comment must be between 6 and 200 symbols";

        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(CommentMaxLength, MinimumLength = CommentMinLength, ErrorMessage = Message)]
        public string Text { get; set; }
    }
}
