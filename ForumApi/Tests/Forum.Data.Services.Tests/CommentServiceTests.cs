using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects;
using Forum.Data.DataTransferObjects.InputModels.Comment;
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
    public class CommentServiceTests
    {
        private readonly IRepository<Post> postRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Comment> commentRepository;
        private readonly IRepository<Category> categoryRepository;
        private readonly ICommentService commentService;

        public CommentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ForumContext(options);

            this.postRepository = new Repository<Post>(context);
            this.userRepository = new Repository<User>(context);
            this.commentRepository = new Repository<Comment>(context);
            this.categoryRepository = new Repository<Category>(context);

            var userStore = new UserStore<User>(context);
            var logger = new Mock<ILogger<CommentService>>();

            var mapperProfile = new MapInitialization();
            var conf = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            var mapper = new Mapper(conf);

            var fakeUserManager = new FakeUserManager(userStore);

            this.commentService = new CommentService(commentRepository, userRepository, postRepository, fakeUserManager, logger.Object, mapper);
        }

        [Fact]
        public async Task ShouldReturnEmptyCollection()
        {
            var all = this.commentRepository.Query().ToList();
            Assert.Empty(all);
        }

        private async Task Seed()
        {
            await this.SeedAdmin();
            await this.SeedCategory();
            await this.SeedPost();
        }

        [Theory]
        [InlineData("admin", "comment text")]
        public async Task CommentCreateSuccessfully(string username, string commentText)
        {
            await this.Seed();
            var post = this.postRepository.Query().ToList().Last().Id;
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var comment = await this.commentService.Create(new CommentInputModel
            {
                PostId = post,
                Text = commentText
            }, username);

            var afterAddCommentsCount = this.commentRepository.Query().ToList().Count();

            var postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);
        }

        [Theory]
        [InlineData("admin", "comment text", 42)]
        public async Task CommentCreateFailInvalidPostId(string username, string commentText, int fakePostId)
        {
            await this.Seed();
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await this.commentService.Create(
                new CommentInputModel
                {
                    PostId = 42,
                    Text = commentText
                }, username));

            var afterFailAddCount= this.commentRepository.Query().ToList().Count;
            Assert.Equal("The provided post for this comment is not valid.", exception.Message);
            Assert.Equal(allBeforeAdd, afterFailAddCount);
        }

        [Theory]
        [InlineData("admin", "comment text", "new comment text")]
        public async Task CommentEditSuccessfully(string username, string commentText, string newText)
        {
            await this.Seed();
            var post = this.postRepository.Query().ToList().Last().Id;
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var comment = await this.commentService.Create(new CommentInputModel
            {
                PostId = post,
                Text = commentText
            }, username);

            var afterAddCommentsCount = this.commentRepository.Query().ToList().Count;

            var postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);

            var commentEditModel = new CommentInputEditModel
            {
                Id = comment.Id,
                Text = newText
            };
            comment = await this.commentService.Edit(commentEditModel, username);

            var afterEditCommentsCount = this.commentRepository.Query().ToList().Count;
            postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(newText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(newText, postComments.First().Text);
            Assert.Equal(username, comment.Author);

            Assert.Equal(afterAddCommentsCount, afterEditCommentsCount);
            Assert.Equal(username, comment.Author);
            Assert.Equal(post, comment.PostId);
        }

        [Theory]
        [InlineData("admin", "comment text", "new comment text", "test", "test@test.com")]
        public async Task CommentEditFailInvalidAuthor(string username, string commentText, string newText, string fakeUsername, string fakeEmail)
        {
            await this.Seed();

            await this.userRepository.AddAsync(new User {UserName = fakeUsername, Email = fakeEmail});
            await this.userRepository.SaveChangesAsync();

            var post = this.postRepository.Query().ToList().Last().Id;
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var comment = await this.commentService.Create(new CommentInputModel
            {
                PostId = post,
                Text = commentText
            }, username);

            var afterAddCommentsCount = this.commentRepository.Query().ToList().Count;

            var postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);

            var commentEditModel = new CommentInputEditModel
            {
                Id = comment.Id,
                Text = newText
            };
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await this.commentService.Edit(commentEditModel, fakeUsername));

            Assert.Equal("You are not allowed for this operation.", exception.Message);

            postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);
        }

        [Theory]
        [InlineData("admin", "comment text", "new text")]
        public async Task CommentEditFailInvalidCommentId(string username, string commentText, string newText)
        {
            await this.Seed();
            var post = this.postRepository.Query().ToList().Last().Id;
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var comment = await this.commentService.Create(new CommentInputModel
            {
                PostId = post,
                Text = commentText
            }, username);

            var afterAddCommentsCount = this.commentRepository.Query().ToList().Count();

            var postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);

            var commentEditModel = new CommentInputEditModel
            {
                Id = 42,
                Text = newText
            };
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await this.commentService.Edit(commentEditModel, username));

            Assert.Equal($"Comment with id '{42}' does not exist.", exception.Message);

            postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);
        }

        [Theory]
        [InlineData("admin", "comment text")]
        public async Task CommentDeleteSuccessfully(string username, string commentText)
        {
            await this.Seed();
            var post = this.postRepository.Query().ToList().Last().Id;
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var comment = await this.commentService.Create(new CommentInputModel
            {
                PostId = post,
                Text = commentText
            }, username);

            var afterAddCommentsCount = this.commentRepository.Query().ToList().Count;

            var postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);

            await this.commentService.Delete(comment.Id, username);

            postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(0, postComments.Count);
        }

        [Theory]
        [InlineData("admin", "comment text", "new text")]
        public async Task CommentDeleteFailInvalidCommentId(string username, string commentText, string newText)
        {
            await this.Seed();
            var post = this.postRepository.Query().ToList().Last().Id;
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var comment = await this.commentService.Create(new CommentInputModel
            {
                PostId = post,
                Text = commentText
            }, username);

            var afterAddCommentsCount = this.commentRepository.Query().ToList().Count();

            var postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);
            var exception = await Assert.ThrowsAsync<Exception>(async () => await this.commentService.Delete(42, username));

            Assert.Equal($"Comment with id '{42}' does not exist.", exception.Message);

            postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);
        }

        [Theory]
        [InlineData("admin", "comment text", "test", "test@test.com")]
        public async Task CommentDeleteFailInvalidAuthor(string username, string commentText, string fakeUsername, string fakeEmail)
        {
            await this.Seed();

            await this.userRepository.AddAsync(new User { UserName = fakeUsername, Email = fakeEmail });
            await this.userRepository.SaveChangesAsync();

            var post = this.postRepository.Query().ToList().Last().Id;
            var allBeforeAdd = this.commentRepository.Query().ToList().Count;

            var comment = await this.commentService.Create(new CommentInputModel
            {
                PostId = post,
                Text = commentText
            }, username);

            var afterAddCommentsCount = this.commentRepository.Query().ToList().Count;

            var postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await this.commentService.Delete(comment.Id, fakeUsername));

            Assert.Equal("You are not allowed for this operation.", exception.Message);

            postComments =
                this.postRepository.Query().Include(p => p.Comments).First(p => p.Id == post).Comments;

            Assert.Equal(1, postComments.Count);
            Assert.Equal(commentText, comment.Text);
            Assert.Equal(comment.Id, postComments.First().Id);
            Assert.Equal(commentText, postComments.First().Text);
            Assert.Equal(allBeforeAdd + 1, afterAddCommentsCount);
            Assert.Equal(username, comment.Author);
        }

        [Fact]
        public async Task ShouldReturn0CommentsForUser()
        {
            var comments = this.commentService.GetCommentsByUsername("testing");

            Assert.Equal(0, comments.Count);
        }

        private async Task SeedPost()
        {
            var categoryId = this.categoryRepository.Query().ToList().First().Id;
            var author = this.userRepository.Query().ToList().First().Id;

            var post = new Post()
            {
                AuthorId = author,
                Title = "Test Title",
                CategoryId = categoryId,
                Body = "Test Description",
            };

            await this.postRepository.AddAsync(post);
            await this.postRepository.SaveChangesAsync();
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
