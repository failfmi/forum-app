using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.WebApi;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests.AccountController
{
    [Collection("Register")]
    public class RegisterTests : IClassFixture<TestingWebApplicationFactory>
    {
        private readonly TestingWebApplicationFactory factory;
        private readonly HttpClient client;

        private const string RegisterEndpoint = "api/account/register";
        private const string EmailTakenMessage = "Email is already taken!";
        private const string UsernameTakenMessage = "Username is already taken!";
        private const string ValidationMessage = "One or more validation errors occurred.";

        public RegisterTests(TestingWebApplicationFactory factory)
        {
            this.factory = factory;

            this.client = this.factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                }).CreateClient();
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test")]
        public async Task RegisterUserSuccessfully(string email, string password, string username)
        {
            var user = new RegisterUserInputModel
            {
                Email = email,
                Password = password,
                Username = username
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");


            var response = await client.PostAsync(RegisterEndpoint, json);
            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "test123")]
        public async Task RegisterFailWithAlreadyTakenEmail(string email, string password, string username)
        {
            var user = new RegisterUserInputModel
            {
                Email = email,
                Password = password,
                Username = username
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(RegisterEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(EmailTakenMessage, content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "admin")]
        public async Task RegisterFailWithAlreadyTakenUsername(string email, string password, string username)
        {
            var user = new RegisterUserInputModel
            {
                Email = email,
                Password = password,
                Username = username
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(RegisterEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(UsernameTakenMessage, content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("test", "12341234", "test")]
        [InlineData("test@test.com", "123", "test")]
        [InlineData("test@test.com", "12341234", "t")]
        public async Task RegisterFailWithInvalidModelData(string email, string password, string username)
        {
            var user = new RegisterUserInputModel
            {
                Email = email,
                Password = password,
                Username = username
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(RegisterEndpoint, json);

            var content = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal(ValidationMessage, content.Title);
        }
    }
}
