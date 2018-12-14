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
    }
}
