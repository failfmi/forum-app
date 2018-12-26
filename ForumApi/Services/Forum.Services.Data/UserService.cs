using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public class UserService : BaseService, IUserService
    {
        private readonly IRepository<User> userRepository;

        public UserService(IRepository<User> userRepository, UserManager<User> userManager, ILogger<BaseService> logger, IMapper mapper) : base(userManager, logger, mapper)
        {
            this.userRepository = userRepository;
        }

        public async Task<UserViewModel> Ban(string id)
        {
            var user = this.GetUserById(id);

            if (user is null)
            {
                throw new ArgumentException($"User with id '{id}' does not exist.");
            }

            if (user.IsActive == false)
            {
                throw new Exception($"User with id '{id}' is already banned!");
            }

            user.IsActive = false;
            this.userRepository.Update(user);
            await this.userRepository.SaveChangesAsync();

            var isAdmin = await this.UserManager.IsInRoleAsync(user, "Admin");
            var mappedUser = this.Mapper.Map<UserViewModel>(user);
            mappedUser.IsAdmin = isAdmin;

            return mappedUser;
        }

        public async Task<UserViewModel> UnBan(string id)
        {
            var user = this.GetUserById(id);

            if (user is null)
            {
                throw new ArgumentException($"User with id '{id}' does not exist.");
            }

            if (user.IsActive == true)
            {
                throw new Exception($"User with id '{id}' is already active!");
            }

            user.IsActive = true;
            this.userRepository.Update(user);
            await this.userRepository.SaveChangesAsync();

            var isAdmin = await this.UserManager.IsInRoleAsync(user, "Admin");
            var mappedUser = this.Mapper.Map<UserViewModel>(user);
            mappedUser.IsAdmin = isAdmin;

            return mappedUser;
        }

        public async Task<ICollection<UserViewModel>> All()
        {
            var users = this.UserManager.Users.ToList();
            var mappedUsers = new List<UserViewModel>();

            foreach (var user in users)
            {
                var isAdmin = await this.UserManager.IsInRoleAsync(user, "Admin");
                var mappedUser = this.Mapper.Map<UserViewModel>(user);
                mappedUser.IsAdmin = isAdmin;
                mappedUsers.Add(mappedUser);
            }

            return mappedUsers;
        }

        private User GetUserById(string id)
        {
            return this.userRepository.Query().FirstOrDefault(u => u.Id == id);
        }
    }
}
