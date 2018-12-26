using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Forum.Data.Models.Users
{
    public class User : IdentityUser
    {
        [Required]
        public bool? IsActive { get; set; }

        [Required]
        public bool? IsLogged { get; set; }

        public DateTime DateRegistered { get; set; }

        public ICollection<LoginInfo> LoginInfo = new HashSet<LoginInfo>();

        public ICollection<Post> Posts = new HashSet<Post>();

        public ICollection<Comment> Comments = new HashSet<Comment>();
    }
}
