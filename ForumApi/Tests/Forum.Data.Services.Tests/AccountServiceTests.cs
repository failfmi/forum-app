using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects;
using Forum.Data.DataTransferObjects.Enums;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.Models.Users;
using Forum.Services.Data;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace Forum.Data.Services.Tests
{
    public class AccountServiceTests
    {
        private readonly IAccountService accountService;
        private readonly IRepository<User> userRepository;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountServiceTests()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            var jwtSettingsSection = config.GetSection("JwtSettings");
            var services = new ServiceCollection();

            services.AddDbContext<ForumContext>(opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddHttpContextAccessor();
            services.AddOptions();
            services.Configure<JwtSettings>(jwtSettingsSection);
            services.AddLogging();
            services.AddAutoMapper(con =>
            {
                con.AddProfile(new MapInitialization());
            });

            var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            var httpContext = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor
            {
                HttpContext = httpContext.Object,
            });

            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ForumContext>()
                .AddDefaultTokenProviders();

            // Add JWT SETTINGS SOMEHOW
            // try to impelement from the nu get package some factory like web tests

            var serviceScope = services.BuildServiceProvider();

            this.accountService = serviceScope.GetService<IAccountService>();
            this.userRepository = serviceScope.GetService<IRepository<User>>();
            this.roleManager = serviceScope.GetService<RoleManager<IdentityRole>>();
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "12341234")]
        public async Task ShouldRegisterSuccessfully(string email, string username, string password)
        {
            await this.SeedRoles();
            var registerModel = new RegisterUserInputModel
            {
                Email = email,
                Username = username,
                Password = password
            };
            await this.accountService.Register(registerModel);

            var users = this.userRepository.Query().ToList();
            var user = users.First();
            Assert.Single(users);
            Assert.Equal(email, user.Email);
            Assert.Equal(username, user.UserName);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "12341234")]
        public async Task ShouldRegisterFailDueToAlreadyExistingUser(string email, string username, string password)
        {
            await this.SeedRoles();
            await this.SeedUser(email, username, password);
            var exception = await Assert.ThrowsAsync<Exception>(async () => await this.accountService.Register(
                new RegisterUserInputModel
                {
                    Email = email,
                    Username = username,
                    Password = password
                }));

            Assert.Equal("Username is already taken!", exception.Message);
            var users = this.userRepository.Query().ToList();
            Assert.Single(users);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "12341234", "test")]
        public async Task ShouldRegisterFailDueToAlreadyExistingEmail(string email, string username, string password, string differentUsername)
        {
            await this.SeedRoles();
            await this.SeedUser(email, username, password);
            var exception = await Assert.ThrowsAsync<Exception>(async () => await this.accountService.Register(
                new RegisterUserInputModel
                {
                    Email = email,
                    Username = differentUsername,
                    Password = password
                }));

            Assert.Equal("Email is already taken!", exception.Message);
            var users = this.userRepository.Query().ToList();
            Assert.Single(users);
        }

        private async Task SeedRoles()
        {
            string[] roleNames = Enum.GetNames(typeof(Roles));

            foreach (string roleName in roleNames)
            {
                //creating the roles and seeding them to the database
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task SeedUser(string email, string username, string password)
        {
            await this.accountService.SeedAdmin(new RegisterUserInputModel
            {
                Email = email,
                Username = username,
                Password = password
            });
        }
    }
}
