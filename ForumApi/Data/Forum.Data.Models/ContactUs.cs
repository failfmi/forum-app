using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Data.Models
{
    public class ContactUs : BaseModel<int>
    {
        public string Email { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }
    }
}
