using System;
using System.Collections.Generic;
using System.Text;
using Forum.Data.Models.Users;

namespace Forum.Data.Models
{
    public class Post : BaseModel<int>
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime CreationDate { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
