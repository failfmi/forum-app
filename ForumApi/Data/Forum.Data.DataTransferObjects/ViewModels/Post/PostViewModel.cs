using System;
using System.Collections.Generic;
using System.Text;
using Forum.Data.DataTransferObjects.ViewModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.Comment;

namespace Forum.Data.DataTransferObjects.ViewModels.Post
{
    public class PostViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreationDate { get; set; }

        public string Author { get; set; }

        public CategoryViewModel Category { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }
    }
}
