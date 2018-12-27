using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Post;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests
{
    public class AdminControllerTests : BaseControllerTest
    {
        private const string AdminBanEndpoint = "api/admin/ban/";
        private const string AdminUnBanEndpoint = "api/admin/unban/";
        private const string AdminGetAll = "api/admin/all";
        private const string PostCreateEndpoint = "api/post/create";

        private const string UnauthorizedError = "You are unauthorized!";

        private const string UnauthorizedMiddlewareError =
            "You are not authorized! Contact admin for further information.";

        public AdminControllerTests(TestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test")]
        public async Task UsersGetAllFailDueToUnauthorizedToken(string email, string password, string username)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(AdminGetAll);


            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
            Assert.Equal(UnauthorizedError, content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234")]
        public async Task UserGetAllFailDueToUnauthorized(string email, string password)
        {
            var response = await this.client.GetAsync(AdminGetAll);
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }


        [Theory]
        [InlineData("admin@admin.com", "12341234", 1)]
        public async Task UserGetAllSuccessful(string email, string password, int expectedUsersCount)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(AdminGetAll);
            var content = JsonConvert.DeserializeObject<ICollection<UserViewModel>>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            Assert.Equal(expectedUsersCount, content.Count);
            Assert.Equal(email, content.First().Email);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "1")]
        public async Task UserBanFailDueToInvalidId(string email, string password, string userId)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(AdminBanEndpoint + userId, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User with id '{userId}' does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "test@test.com", "12341234", "test")]
        public async Task UserBanSuccessfully(string email, string password, string bannedUserEmail,
            string bannedUserPassword, string bannedUserUsername)
        {
            await this.Register(bannedUserEmail, bannedUserPassword, bannedUserUsername);

            var token = await this.Login(email, password);
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(AdminGetAll);
            var content = JsonConvert.DeserializeObject<ICollection<UserViewModel>>(await response.Content.ReadAsStringAsync());
            var secondUser = content.First(uvm => uvm.Email == bannedUserEmail);

            response.EnsureSuccessStatusCode();

            Assert.Equal(email, content.First().Email);
            Assert.Equal(bannedUserEmail, secondUser.Email);
            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            response = await this.client.PostAsync(AdminBanEndpoint + secondUser.Id, json);

            var secondResponseContent = JsonConvert.DeserializeObject<CreateEditReturnMessage<UserViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User banned successfully", secondResponseContent.Message);
            Assert.Equal(StatusCodes.Status200OK, secondResponseContent.Status);

            var user = new LoginUserInputModel
            {
                Email = bannedUserEmail,
                Password = bannedUserPassword,
            };

            json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var responseBannedLogin = await this.client.PostAsync("api/account/login", json);
            var thirdResponseContent = JsonConvert.DeserializeObject<ReturnMessage>(await responseBannedLogin.Content.ReadAsStringAsync());

            Assert.Equal("You are banned! Contact admin for further information.", thirdResponseContent.Message);
            Assert.Equal(StatusCodes.Status401Unauthorized, thirdResponseContent.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "test@test.com", "12341234", "test")]
        public async Task UserBanFailDueToAlreadyBannedUser(string email, string password, string bannedUserEmail,
            string bannedUserPassword, string bannedUserUsername)
        {
            await this.Register(bannedUserEmail, bannedUserPassword, bannedUserUsername);

            var token = await this.Login(email, password);
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(AdminGetAll);
            var content = JsonConvert.DeserializeObject<ICollection<UserViewModel>>(await response.Content.ReadAsStringAsync());
            var secondUser = content.First(uvm => uvm.Email == bannedUserEmail);

            response.EnsureSuccessStatusCode();

            Assert.Equal(email, content.First().Email);
            Assert.Equal(bannedUserEmail, secondUser.Email);
            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            response = await this.client.PostAsync(AdminBanEndpoint + secondUser.Id, json);

            var secondResponseContent = JsonConvert.DeserializeObject<CreateEditReturnMessage<UserViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User banned successfully", secondResponseContent.Message);
            Assert.Equal(StatusCodes.Status200OK, secondResponseContent.Status);

            response = await this.client.PostAsync(AdminBanEndpoint + secondUser.Id, json);

            var thirdResponseContent = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User with id '{secondUser.Id}' is already banned!", thirdResponseContent.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, thirdResponseContent.Status);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test")]
        public async Task UsersBanFailDueToUnauthorizedToken(string email, string password, string username)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(AdminBanEndpoint + password, json);
            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
            Assert.Equal(UnauthorizedError, content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234")]
        public async Task UserBanFailDueToUnauthorized(string email, string password)
        {
            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(AdminBanEndpoint + password, json);
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234")]
        public async Task UserUnBanFailDueToUnauthorized(string email, string password)
        {
            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(AdminUnBanEndpoint + password, json);
            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test")]
        public async Task UsersUnBanFailDueToUnauthorizedToken(string email, string password, string username)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(AdminUnBanEndpoint + password, json);
            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
            Assert.Equal(UnauthorizedError, content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "1")]
        public async Task UserUnBanFailDueToInvalidId(string email, string password, string userId)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(AdminUnBanEndpoint + userId, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User with id '{userId}' does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "test@test.com", "12341234", "test")]
        public async Task UserUnBanSuccessfully(string email, string password, string bannedUserEmail,
            string bannedUserPassword, string bannedUserUsername)
        {
            await this.Register(bannedUserEmail, bannedUserPassword, bannedUserUsername);

            var token = await this.Login(email, password);
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(AdminGetAll);
            var content = JsonConvert.DeserializeObject<ICollection<UserViewModel>>(await response.Content.ReadAsStringAsync());
            var secondUser = content.First(uvm => uvm.Email == bannedUserEmail);

            response.EnsureSuccessStatusCode();

            Assert.Equal(email, content.First().Email);
            Assert.Equal(bannedUserEmail, secondUser.Email);
            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            response = await this.client.PostAsync(AdminBanEndpoint + secondUser.Id, json);

            var secondResponseContent = JsonConvert.DeserializeObject<CreateEditReturnMessage<UserViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User banned successfully", secondResponseContent.Message);
            Assert.Equal(StatusCodes.Status200OK, secondResponseContent.Status);

            var user = new LoginUserInputModel
            {
                Email = bannedUserEmail,
                Password = bannedUserPassword,
            };

            json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var responseBannedLogin = await this.client.PostAsync("api/account/login", json);
            var thirdResponseContent = JsonConvert.DeserializeObject<ReturnMessage>(await responseBannedLogin.Content.ReadAsStringAsync());

            Assert.Equal("You are banned! Contact admin for further information.", thirdResponseContent.Message);
            Assert.Equal(StatusCodes.Status401Unauthorized, thirdResponseContent.Status);

            response = await this.client.PostAsync(AdminUnBanEndpoint + secondUser.Id, json);

            var fourthResponse = JsonConvert.DeserializeObject<CreateEditReturnMessage<UserViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User unbanned successfully", fourthResponse.Message);
            Assert.Equal(StatusCodes.Status200OK, fourthResponse.Status);

            await this.Login(bannedUserEmail, bannedUserPassword);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "test@test.com", "12341234", "test")]
        public async Task UserUnBanFailDueToAlreadyActiveUser(string email, string password, string bannedUserEmail,
            string bannedUserPassword, string bannedUserUsername)
        {
            await this.Register(bannedUserEmail, bannedUserPassword, bannedUserUsername);

            var token = await this.Login(email, password);
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(AdminGetAll);
            var content =
                JsonConvert.DeserializeObject<ICollection<UserViewModel>>(await response.Content.ReadAsStringAsync());
            var secondUser = content.First(uvm => uvm.Email == bannedUserEmail);

            response.EnsureSuccessStatusCode();

            Assert.Equal(email, content.First().Email);
            Assert.Equal(bannedUserEmail, secondUser.Email);
            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            await this.client.PostAsync(AdminBanEndpoint + secondUser.Id, json);
            await this.client.PostAsync(AdminUnBanEndpoint + secondUser.Id, json);

            response = await this.client.PostAsync(AdminUnBanEndpoint + secondUser.Id, json);

            var secondResponseContent = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());
            Assert.Equal($"User with id '{secondUser.Id}' is already active!", secondResponseContent.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, secondResponseContent.Status);
        }


        [Theory]
        [InlineData("admin@admin.com", "12341234", "test@test.com", "12341234", "test", 1, "Test Title", "Test Body with many letters")]
        public async Task UserLoggedInBannedTryingToCreateAPost(string email, string password, string bannedUserEmail,
            string bannedUserPassword, string bannedUserUsername, int categoryId, string title, string body)
        {
            await this.Register(bannedUserEmail, bannedUserPassword, bannedUserUsername);

            var bannedUserToken = await this.Login(bannedUserEmail, bannedUserPassword);
            var token = await this.Login(email, password);
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(AdminGetAll);
            var content = JsonConvert.DeserializeObject<ICollection<UserViewModel>>(await response.Content.ReadAsStringAsync());
            var secondUser = content.First(uvm => uvm.Email == bannedUserEmail);

            response.EnsureSuccessStatusCode();

            Assert.Equal(email, content.First().Email);
            Assert.Equal(bannedUserEmail, secondUser.Email);
            var json = new StringContent(
                JsonConvert.SerializeObject("{}"),
                Encoding.UTF8,
                "application/json");

            response = await this.client.PostAsync(AdminBanEndpoint + secondUser.Id, json);

            var secondResponseContent = JsonConvert.DeserializeObject<CreateEditReturnMessage<UserViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"User banned successfully", secondResponseContent.Message);
            Assert.Equal(StatusCodes.Status200OK, secondResponseContent.Status);

            var post = new PostInputModel
            {
                CategoryId = categoryId,
                Title = title,
                Body = body
            };

            json = new StringContent(
                JsonConvert.SerializeObject(post),
                Encoding.UTF8,
                "application/json");

            this.client.DefaultRequestHeaders.Remove("Authorization");
            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bannedUserToken);

            var responseBannedCreatePost= await this.client.PostAsync(PostCreateEndpoint, json);
            var responseContent = JsonConvert.DeserializeObject<ReturnMessage>(await responseBannedCreatePost.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, responseContent.Status);
            Assert.Equal(UnauthorizedMiddlewareError, responseContent.Message);
        }
    }
}
