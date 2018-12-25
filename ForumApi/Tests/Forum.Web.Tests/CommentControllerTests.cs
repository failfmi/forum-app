using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Comment;
using Forum.Data.DataTransferObjects.ViewModels.Comment;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests
{
    public class CommentControllerTests : BaseControllerTest
    {
        private const string CommentCreateEndpoint = "api/comment/create";
        private const string CommentEditEndpoint = "api/comment/edit/";
        private const string CommentDeleteEndpoint = "api/comment/delete/";
        private const string CommentsGetByUsername = "api/comment/get/";

        public CommentControllerTests(TestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1, "Test Body with many letters")]
        public async Task CreateCommentSuccessfully(string email, string password, int postId,
            string body)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputModel
            {
                PostId = postId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(CommentCreateEndpoint, json);

            response.EnsureSuccessStatusCode();

            var content = JsonConvert.DeserializeObject<CreateEditReturnMessage<CommentViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status200OK, content.Status);
            Assert.Equal("Comment created successfully!", content.Message);
            Assert.Equal(body, content.Data.Text);
            Assert.Equal(email.Split("@")[0], content.Data.Author);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 42, "Test Body with many letters")]
        public async Task CreateCommentFailDueToInvalidPostId(string email, string password, int postId, string body)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputModel
            {
                PostId = postId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(CommentCreateEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal("The provided post for this comment is not valid.", content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", null, null)]
        [InlineData("admin@admin.com", "12341234", 1, "Test")]
        public async Task CreateCommentFailDudeToInvalidParameters(string email, string password, int postId,  string body)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputModel
            {
                PostId = postId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(CommentCreateEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal(ValidationMessage, content.Title);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1, "Testing body")]
        public async Task EditCommentSuccessfully(string email, string password, int commentId, string body)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputEditModel
            {
                Id = commentId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PutAsync(CommentEditEndpoint + commentId, json);

            response.EnsureSuccessStatusCode();

            var content = JsonConvert.DeserializeObject<CreateEditReturnMessage<CommentViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status200OK, content.Status);
            Assert.Equal("Comment edited successfully", content.Message);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test", 1, "Testing body")]
        public async Task EditCommentFailInvalidAuthor(string email, string password, string username, int commentId, string body)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputEditModel
            {
                Id = commentId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PutAsync(CommentEditEndpoint + commentId, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
            Assert.Equal("You are not allowed for this operation.", content.Message);
        }


        [Theory]
        [InlineData("admin@admin.com", "12341234", 42, "Testing body")]
        public async Task EditCommentFailInvalidCommentId(string email, string password, int commentId, string body)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputEditModel
            {
                Id = commentId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PutAsync(CommentEditEndpoint + commentId, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal($"Comment with id '{commentId}' does not exist.", content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1, 2, "Testing body")]
        public async Task EditCommentFailDueToIds(string email, string password, int commentId, int fakeCommentId, string body)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputEditModel
            {
                Id = commentId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PutAsync(CommentEditEndpoint + fakeCommentId, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal("Invalid ids", content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", null, null)]
        public async Task EditCommentFailDueToInvalidParameters(string email, string password, int commentId, string body)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var post = new CommentInputEditModel
            {
                Id = commentId,
                Text = body
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PutAsync(CommentEditEndpoint + commentId, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal(ValidationMessage, content.Title);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 8)]
        [InlineData("admin@admin.com", "12341234", 42)]
        public async Task DeleteCommentFailDueToInvalidId(string email, string password, int id)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.DeleteAsync(CommentDeleteEndpoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Comment with id '{id}' does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 2)]
        public async Task DeleteCommentSuccessfully(string email, string password, int id)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.DeleteAsync(CommentDeleteEndpoint + id);

            response.EnsureSuccessStatusCode();

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal("Comment deleted successfully!", content.Message);
            Assert.Equal(StatusCodes.Status200OK, content.Status);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test", 2)]
        public async Task DeleteCommentFailDueToInvalidAuthor(string email, string password, string username, int id)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.DeleteAsync(CommentDeleteEndpoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal("You are not allowed for this operation.", content.Message);
            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
        }


        [Theory]
        [InlineData("admin", 3)]
        [InlineData("test", 0)]
        public async Task GetCommentsByUser(string username, int expectedCommentsCount)
        {
            var response = await this.client.GetAsync(CommentsGetByUsername + username);
            var content = JsonConvert.DeserializeObject<IEnumerable<CommentViewModel>>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            Assert.Equal(expectedCommentsCount, content.Count());
        }
    }
}
