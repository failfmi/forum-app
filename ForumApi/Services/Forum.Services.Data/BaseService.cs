using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Forum.Data;
using Forum.Data.Common.Interfaces;
using Forum.Data.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public abstract class BaseService
    {
        protected BaseService(UserManager<User> userManager, ILogger<BaseService> logger,
            IMapper mapper)
        {
            this.UserManager = userManager;
            this.Logger = logger;
            this.Mapper = mapper;
        }

        protected UserManager<User> UserManager { get; set; }

        protected ILogger Logger { get; set; }

        protected IMapper Mapper { get; set; }
    }
}
