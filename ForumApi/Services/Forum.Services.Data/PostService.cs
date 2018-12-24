﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.InputModels.Post;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.Post;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public class PostService : BaseService, IPostService
    {
        private readonly IRepository<Post> postRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<User> userRepository;
        
        public PostService(IRepository<Post> postRepository, IRepository<Category> categoryRepository, IRepository<User> userRepository, UserManager<User> userManager, ILogger<BaseService> logger, IMapper mapper) : base(userManager, logger, mapper)
        {
            this.postRepository = postRepository;
            this.categoryRepository = categoryRepository;
            this.userRepository = userRepository;
        }

        public async Task<PostViewModel> Create(PostInputModel model, string email)
        {
            if (!this.IsValidPostTitle(model.Title))
            {
                throw new ArgumentException($"Post with title '{model.Title}' already exists");
            }

            if (!this.IsValidCategory(model.CategoryId))
            {
                throw new ArgumentException("The category provided for the creation of this post is not valid!");
            }

            var user = this.userRepository.Query().FirstOrDefault(u => u.Email == email);

            var post = this.Mapper.Map<Post>(model);
            post.CreationDate = DateTime.UtcNow;
            post.Author = user;

            await this.postRepository.AddAsync(post);
            await this.postRepository.SaveChangesAsync();

            post = this.postRepository.Query().Include(p => p.Category).FirstOrDefault(p => p.Id == post.Id);

            return this.Mapper.Map<Post, PostViewModel>(post);
        }

        public async Task<PostViewModel> Edit(PostInputEditModel model, string email)
        {
            if (!this.IsValidId(model.Id))
            {
                throw new ArgumentException($"Post with id '{model.Id}' does not exist");
            }

            var user = this.userRepository.Query().FirstOrDefault(u => u.Email == email);

            var postOwner = this.postRepository.Query().FirstOrDefault(p => p.Id == model.Id)?.Author.Email;
            var isCallerAdmin = await this.UserManager.IsInRoleAsync(user, "Admin");
            if (postOwner != user?.Email || !isCallerAdmin)
            {
                throw new UnauthorizedAccessException("You are not allowed for this operation.");
            }

            var post = this.Mapper.Map<Post>(model);

            this.postRepository.Update(post);

            post = this.postRepository.Query().Include(p => p.Category).FirstOrDefault(p => p.Id == post.Id);

            return this.Mapper.Map<PostViewModel>(post);
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public ICollection<PostViewModel> All()
        {
            var posts = this.postRepository
                .Query()
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .ToList();

            return this.Mapper.Map<ICollection<PostViewModel>>(posts);
        }

        private bool IsValidCategory(int id)
        {
            return this.categoryRepository.Query().Count(c => c.Id == id) != 0;
        }

        private bool IsValidId(int id)
        {
            return this.postRepository.Query().Count(p => p.Id == id) != 0;
        }

        private bool IsValidPostTitle(string title)
        {
            return this.postRepository.Query().Count(p => p.Title.ToLower().Trim() == title.ToLower().Trim()) == 0;
        }
    }
}