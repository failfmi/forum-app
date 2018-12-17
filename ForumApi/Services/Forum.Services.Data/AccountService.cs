using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.Enums;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Forum.Services.Data
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly SignInManager<User> signInManager;
        private readonly IRepository<User> userRepository;
        private readonly JwtSettings jwtSettings;

        public AccountService(UserManager<User> userManager, IOptions<JwtSettings> jwtSettings, ILogger<BaseService> logger, IMapper mapper, IRepository<User> userRepository, SignInManager<User> signInManager)
            : base(userManager, logger, mapper)
        {
            this.userRepository = userRepository;
            this.signInManager = signInManager;
            this.jwtSettings = jwtSettings.Value;
        }

        public async Task Register(RegisterUserInputModel model)
        {
            var user = this.Mapper.Map<RegisterUserInputModel, User>(model);

            if (this.UserManager.Users.Count(u => u.UserName == user.UserName) != 0)
            {
                throw new Exception("Username is already taken!");
            }

            if (this.UserManager.Users.Count(u => u.Email == user.Email) != 0)
            {
                throw new Exception("Email is already taken!");
            }

            await this.UserManager.CreateAsync(user, model.Password);
            await this.UserManager.AddToRoleAsync(user, Enum.GetName(typeof(Roles), 2));
        }

        public async Task<string> Login(LoginUserInputModel model)
        {
            var user = this.UserManager.Users.SingleOrDefault(u => u.Email == model.Email);
            if (user is null)
            {
                throw new Exception("Invalid username or password!");
            }

            var result = await this.signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                throw new Exception("Invalid username or password!");
            }

            return GenerateToken(user);
        }

        public async Task SeedAdmin(RegisterUserInputModel model)
        {
            var user = this.Mapper.Map<RegisterUserInputModel, User>(model);
            user.IsActive = true;

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

        protected string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("isAdmin", this.UserManager.IsInRoleAsync(user, "Admin").GetAwaiter().GetResult().ToString(), ClaimValueTypes.Boolean),
                    new Claim("isBanned", (!user.IsActive).ToString(), ClaimValueTypes.Boolean)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
