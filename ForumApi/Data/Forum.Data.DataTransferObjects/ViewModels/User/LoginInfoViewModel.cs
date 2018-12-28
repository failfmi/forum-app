using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Data.DataTransferObjects.ViewModels.User
{
    public class LoginInfoViewModel
    {
        public string Ip { get; set; }

        public string Location { get; set; }

        public DateTime LoginDate { get; set; }
    }
}
