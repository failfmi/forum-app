using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects;
using Forum.Data.DataTransferObjects.InputModels.Post;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Data.Services.Tests.Fake;
using Forum.Services.Data;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Forum.Data.Services.Tests
{
    [TestCaseOrderer("Forum.Data.Services.Tests.PriorityOrder.PriorityOrderer", "Forum.Data.Services.Tests")]
    public class PostServiceTests
    {
        private readonly IRepository<Category> categoryRepository;
        private readonly IRepository<Post> postRepository;
        private readonly IRepository<User> userRepository;
        private readonly IPostService postService;

        public PostServiceTests()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ForumContext(options);

            this.postRepository = new Repository<Post>(context);
            this.categoryRepository = new Repository<Category>(context);
            this.userRepository = new Repository<User>(context);

            var userStore = new UserStore<User>(context);
            var logger = new Mock<ILogger<PostService>>();

            var mapperProfile = new MapInitialization();
            var conf = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            var mapper = new Mapper(conf);

            var fakeUserManager = new FakeUserManager(userStore);

            this.postService = new PostService(postRepository, categoryRepository, userRepository, fakeUserManager, logger.Object, mapper);
        }

        [Fact]
        public async Task ShouldReturnEmptyCollection()
        {
            var all = this.postService.All();
            Assert.Empty(all);
        }

        [Theory]
        [InlineData(0)]
        public async Task ShouldFailGetPostById(int id)
        {
            var exception = Assert.Throws<Exception>(() => this.postService.GetPostById(id));
            Assert.Equal($"Post with id {id} does not exist.", exception.Message);
        }

        [Theory]
        [InlineData(42, "Test Title", "Test description")]
        public async Task ShouldFailEditInvalidId(int id, string title, string body)
        {
            await this.Seed();

            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await this.postService.Edit(new PostInputEditModel { Id = id, Title = title, Body = body, CategoryId = categoryId }, "admin@admin.com"));

            Assert.Equal($"Post with id '{id}' does not exist", exception.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "test title", "test description")]
        public async Task CreatePostSuccessfully(string email, string username, string title, string description)
        {
            await this.Seed();

            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var postModel = new PostInputModel
            {
                CategoryId = categoryId,
                Title = title,
                Body = description
            };

            var post = await this.postService.Create(postModel, email);

            Assert.Equal(title, post.Title);
            Assert.Equal(username, post.Author);
            Assert.Equal(description, post.Body);
            Assert.Equal(categoryId, post.Category.Id);
            Assert.Equal(0, post.Comments.Count);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "test title", "test description")]
        public async Task CreatePostFailDueToSameTitle(string email, string username, string title, string description)
        {
            await this.Seed();

            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var postModel = new PostInputModel
            {
                CategoryId = categoryId,
                Title = title,
                Body = description
            };

            var post = await this.postService.Create(postModel, email);

            var exception =
                await Assert.ThrowsAsync<ArgumentException>(async () => await this.postService.Create(postModel, email));

            Assert.Equal($"Post with title '{title}' already exists", exception.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "test title", "test description")]
        public async Task CreatePostFailDueToInvalidCategoryId(string email, string username, string title, string description)
        {
            await this.Seed();

            var postModel = new PostInputModel
            {
                CategoryId = 42,
                Title = title,
                Body = description
            };

            var exception =
                await Assert.ThrowsAsync<ArgumentException>(async () => await this.postService.Create(postModel, email));

            Assert.Equal("The category provided for the creation of this post is not valid!", exception.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "test title", "test description", "test title 2", "test desc 2")]
        public async Task EditSuccessfully(string email, string username, string title, string description, string newTitle, string newDescription)
        {
            await this.Seed();

            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var postModel = new PostInputEditModel
            {
                Id = 1,
                CategoryId = categoryId,
                Title = title,
                Body = description
            };

            await this.postService.Create(postModel, email);

            postModel.Title = newTitle;
            postModel.Body = newDescription;

            var edittedPost = await this.postService.Edit(postModel, email);

            var post = this.postService.GetPostById(edittedPost.Id);

            Assert.Equal(newTitle, post.Title);
            Assert.Equal(newDescription, post.Body);
            Assert.Equal(username, post.Author);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "test title", "test description", "test title 2", "test desc 2")]
        public async Task EditFailInvalidCategoryId(string email, string username, string title, string description, string newTitle, string newDescription)
        {
            await this.Seed();

            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var postModel = new PostInputEditModel
            {
                Id = 1,
                CategoryId = categoryId,
                Title = title,
                Body = description
            };

            await this.postService.Create(postModel, email);

            postModel.CategoryId = 42;
            postModel.Title = newTitle;
            postModel.Body = newDescription;

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await this.postService.Edit(postModel, email));
            Assert.Equal("The category provided for the edit of this post is not valid!", exception.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "admin", "test title", "test description", "test title 2", "test desc 2")]
        public async Task EditFailInvalidAuthor(string email, string username, string title, string description, string newTitle, string newDescription)
        {
            await this.Seed();
            await this.userRepository.AddAsync(new User { UserName = "test", Email = "test@test.com" });
            await this.userRepository.SaveChangesAsync();

            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var postModel = new PostInputEditModel
            {
                Id = 1,
                CategoryId = categoryId,
                Title = title,
                Body = description
            };

            await this.postService.Create(postModel, "test@test.com");

            postModel.CategoryId = categoryId;
            postModel.Title = newTitle;
            postModel.Body = newDescription;

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await this.postService.Edit(postModel, email));
            Assert.Equal("You are not allowed for this operation.", exception.Message);
        }

        [Theory]
        [InlineData("test title", "test description", "admin@admin.com")]
        public async Task ShouldDeleteSuccessfully(string title, string description, string email)
        {
            await this.Seed();
            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var postModel = new PostInputEditModel
            {
                Id = 1,
                CategoryId = categoryId,
                Title = title,
                Body = description
            };

            var post = await this.postService.Create(postModel, email);

            var all = this.postService.All();

            var deleted = await this.postService.Delete(post.Id, email);

            var afterDeleted = this.postService.All();

            Assert.Equal(all.Count, afterDeleted.Count + 1);
            Assert.Equal(deleted, title);
        }

        [Theory]
        [InlineData("test title", "test description", "admin@admin.com")]
        public async Task PostDeleteFailInvalidId(string title, string description, string email)
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await this.postService.Delete(42, email));

            Assert.Equal($"Post with id '{42}' does not exist", exception.Message);
        }


        [Theory]
        [InlineData("test title", "test description", "test", "test@test.com", "admin@admin.com")]
        public async Task PostDeleteFailDueToUnauthorized(string title, string description, string invalidUser,
            string invalidEmail, string actualEmail)
        {
            await this.Seed();
            await this.userRepository.AddAsync(new User { UserName = "test", Email = "test@test.com" });
            await this.userRepository.SaveChangesAsync();

            var categoryId = this.categoryRepository.Query().ToList().Last().Id;
            var postModel = new PostInputEditModel
            {
                Id = 1,
                CategoryId = categoryId,
                Title = title,
                Body = description
            };

            var post = await this.postService.Create(postModel, "test@test.com");

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await this.postService.Delete(post.Id, actualEmail));
            Assert.Equal("You are not allowed for this operation.", exception.Message);
        }

        private async Task Seed()
        {
            await this.SeedAdmin();
            await this.SeedCategory();
        }

        private async Task SeedAdmin()
        {
            await this.userRepository.AddAsync(new User { UserName = "admin", Email = "admin@admin.com" });
            await this.userRepository.SaveChangesAsync();
        }

        private async Task SeedCategory()
        {
            await this.categoryRepository.AddAsync(new Category { Name = "Programming" });
            await this.categoryRepository.SaveChangesAsync();
        }
    }
}
