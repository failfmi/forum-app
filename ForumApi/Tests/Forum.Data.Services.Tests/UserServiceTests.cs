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
    public class UserServiceTests
    {
        private readonly IUserService userService;
        private readonly IRepository<User> userRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IAccountService accountService;

        public UserServiceTests()
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

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();

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


            var httpContext = new Mock<HttpContext>();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor
            {
                HttpContext = httpContext.Object,
            });

            // Add JWT SETTINGS SOMEHOW
            // try to impelement from the nu get package some factory like web tests

            var serviceScope = services.BuildServiceProvider();

            this.userService = serviceScope.GetService<IUserService>();
            this.userRepository = serviceScope.GetService<IRepository<User>>();
            this.roleManager = serviceScope.GetService<RoleManager<IdentityRole>>();
            this.userManager = serviceScope.GetService<UserManager<User>>();
            this.accountService = serviceScope.GetService<IAccountService>();
        }

        [Fact]
        public async Task ShouldHaveEmptyCollection()
        {
            var users = await this.userRepository.Query().ToListAsync();

            Assert.Empty(users);
        }

        [Fact]
        public async Task ShouldGetUsersSuccessfully()
        {
            await this.Seed();
            var users = await this.userService.All();
            Assert.Single(users);
        }

        [Fact]
        public async Task ShouldBanUserSuccessfully()
        {
            await this.Seed();

            var users = await this.userService.All();
            Assert.Single(users);
            var toBeBannedUser = users.First();
            Assert.False(toBeBannedUser.IsBanned);

            await this.userService.Ban(toBeBannedUser.Id);
            users = await this.userService.All();

            var bannedUser = users.Last();
            Assert.True(bannedUser.IsBanned);
            Assert.Single(users);
        }

        [Theory]
        [InlineData("42")]
        public async Task ShouldBanUserFailDueToInvalidId(string userId)
        {
            await this.Seed();
            
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await this.userService.Ban(userId));

            var users = this.userRepository.Query().ToList();
            Assert.Equal($"User with id '{userId}' does not exist.", exception.Message);
            Assert.Single(users);
        }

        [Fact]
        public async Task ShouldBanUserFailDueToAlreadyBanned()
        {
            await this.Seed();

            var users = await this.userService.All();
            var toBeBannedUser = users.Last();
            Assert.False(toBeBannedUser.IsBanned);

            await this.userService.Ban(toBeBannedUser.Id);
            users = await this.userService.All();

            var bannedUser = users.Last();
            Assert.True(bannedUser.IsBanned);
            Assert.Single(users);

            var exception = await Assert.ThrowsAsync<Exception>(async () => await this.userService.Ban(bannedUser.Id));
            
            Assert.Equal($"User with id '{bannedUser.Id}' is already banned!", exception.Message);
        }

        [Theory]
        [InlineData("42")]
        public async Task ShouldUnBanUserFailDueToInvalidId(string userId)
        {
            await this.Seed();

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await this.userService.UnBan(userId));

            var users = this.userRepository.Query().ToList();
            Assert.Equal($"User with id '{userId}' does not exist.", exception.Message);
            Assert.Single(users);
        }

        [Fact]
        public async Task ShouldUnBanUserFailDueToAlreadyActive()
        {
            await this.Seed();
            var users = this.userRepository.Query().ToList();
            var user = users.First();
            var exception = await Assert.ThrowsAsync<Exception>(async () => await this.userService.UnBan(user.Id));

            users = this.userRepository.Query().ToList();
            Assert.Equal($"User with id '{user.Id}' is already active!", exception.Message);
            Assert.Single(users);
        }

        [Fact]
        public async Task ShouldUnBanUserSuccessfully()
        {
            await this.Seed();

            var users = await this.userService.All();
            var toBeBannedUser = users.Last();
            Assert.False(toBeBannedUser.IsBanned);

            await this.userService.Ban(toBeBannedUser.Id);
            users = await this.userService.All();

            var bannedUser = users.Last();
            Assert.True(bannedUser.IsBanned);
            Assert.Single(users);

            await this.userService.UnBan(bannedUser.Id);
            users = await this.userService.All();

            Assert.False(users.Last().IsBanned);
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

        private async Task Seed()
        {
            await this.SeedRoles();
            await this.accountService.SeedAdmin(new RegisterUserInputModel
            {
                Email = "admin@admin.com",
                Password = "12341234",
                Username = "admin"
            });
        }
    }
}
