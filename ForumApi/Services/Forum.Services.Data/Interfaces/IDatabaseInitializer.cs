using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.Common.Interfaces;
using Forum.Data.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data.Interfaces
{
    public interface IDatabaseInitializer
    {
        Task Seed(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration, IAccountService accountService, ILogger<IDatabaseInitializer> logger, IRepository<User> userRepository);
    }
}
