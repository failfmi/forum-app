using System;

namespace Forum.Data.DataTransferObjects.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public int PostId { get; set; }

        public string Author { get; set; }
    }
}
