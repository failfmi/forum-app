using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.Enums;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly SignInManager<User> signInManager;
        private readonly IRepository<User> userRepository;

        public AccountService(UserManager<User> userManager, ILogger<BaseService> logger, IMapper mapper, IRepository<User> userRepository, SignInManager<User> signInManager) : base(userManager, logger, mapper)
        {
            this.userRepository = userRepository;
            this.signInManager = signInManager;
        }

        public async Task<bool> Register(RegisterUserInputModel model)
        {
            var user = this.Mapper.Map<RegisterUserInputModel, User>(model);

            var result = false;
            try
            {
                await this.UserManager.CreateAsync(user, model.Password);
                result = this.UserManager.AddToRoleAsync(user, Enum.GetName(typeof(Roles), 2)).GetAwaiter().GetResult().Succeeded;
            }
            catch (Exception e)
            {
                this.Logger.LogWarning(e, "Creation of ApplicationUser and its role resulted in failure: " + e.Message);
            }

            return result;
        }

        public async Task SeedAdmin(RegisterUserInputModel model)
        {
            var user = this.Mapper.Map<RegisterUserInputModel, User>(model);

            try
            {
                await this.UserManager.CreateAsync(user, model.Password);

                await this.UserManager.AddToRoleAsync(user, Enum.GetName(typeof(Roles), 1));
            }
            catch (Exception e)
            {
                this.Logger.LogWarning(e, "Creation of ApplicationUser and its role resulted in failure: " + e.Message);
            }
        }
    }
}
