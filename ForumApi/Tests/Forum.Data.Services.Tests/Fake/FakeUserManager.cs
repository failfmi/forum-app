using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Forum.Data.Services.Tests.Fake
{

    public class FakeUserManager : UserManager<User>
    {
        public FakeUserManager(UserStore<User> userStore)
            : base(userStore, null, null, null, null, null, null, null, null)
        { }
    }
}
