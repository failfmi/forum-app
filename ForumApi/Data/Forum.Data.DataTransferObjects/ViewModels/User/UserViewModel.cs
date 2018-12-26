using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Data.DataTransferObjects.ViewModels.User
{
    public class UserViewModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public DateTime DateRegistered { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsBanned { get; set; }
    }
}
