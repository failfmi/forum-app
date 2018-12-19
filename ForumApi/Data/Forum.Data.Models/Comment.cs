using System;
using System.Collections.Generic;
using System.Text;
using Forum.Data.Models.Users;

namespace Forum.Data.Models
{
    public class Comment : BaseModel<int>
    {
        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }
    }
}
