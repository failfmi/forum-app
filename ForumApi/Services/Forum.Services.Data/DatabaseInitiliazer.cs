using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.Enums;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public class DatabaseInitiliazer : IDatabaseInitializer
    {
        public async Task Seed(RoleManager<IdentityRole> roleManager, UserManager<User> userManager,
            IConfiguration configuration, IAccountService accountService, ILogger<IDatabaseInitializer> logger, IRepository<User> userRepository, IRepository<Category> categoryRepository)
        {
            await this.ConfigureUserRoles(roleManager, logger);

            await this.SeedUser(userRepository, configuration, accountService, logger);

            await this.SeedCategories(categoryRepository, logger);
        }

        private async Task ConfigureUserRoles(RoleManager<IdentityRole> roleManager, ILogger<IDatabaseInitializer> logger)
        {
            //Adding custom roles
            string[] roleNames = Enum.GetNames(typeof(Roles));

            foreach (string roleName in roleNames)
            {
                //creating the roles and seeding them to the database
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    logger.LogWarning("Start Seeding User Roles...");

                    await roleManager.CreateAsync(new IdentityRole(roleName));

                    logger.LogWarning("End Seeding User Roles Successfully");
                }
            }
        }

        private async Task SeedUser(IRepository<User> userRepository, IConfiguration configuration, IAccountService accountService, ILogger<IDatabaseInitializer> logger)
        {
            string username = configuration.GetSection("UserSettings")["Username"];

            if (!userRepository.Query().Any(u => u.UserName == username))
            {
                logger.LogWarning("Start Seeding User...");

                var adminModel = new RegisterUserInputModel()
                {
                    Username = configuration.GetSection("UserSettings")["Username"],
                    Email = configuration.GetSection("UserSettings")["Email"],
                    Password = configuration.GetSection("UserSettings")["Password"],
                };

                await accountService.SeedAdmin(adminModel);

                logger.LogWarning("End Seeding User Successfully");
            }
        }

        private async Task SeedCategories(IRepository<Category> categoryRepository, ILogger<IDatabaseInitializer> logger)
        {
            var categories = new string[]
                {"Education", "Football", "Basketball", "Marketing", "Blockchain", "Programming", "Game Theory"};
            if (!categoryRepository.Query().Any())
            {
                logger.LogWarning("Start Seeding Categories...");
                foreach (var category in categories)
                {
                    await categoryRepository.AddAsync(new Category { Name = category });
                }

                await categoryRepository.SaveChangesAsync();

                logger.LogWarning("End Seeding categories Successfully");
            }
        }
    }
}
