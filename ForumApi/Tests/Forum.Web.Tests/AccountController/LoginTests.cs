using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests.AccountController
{
    [Collection("Login")]
    public class LoginTests : IClassFixture<TestingWebApplicationFactory>
    {
        private readonly TestingWebApplicationFactory factory;
        private readonly HttpClient client;

        private const string LoginEndpoint = "api/account/login";
        private const string LoginErrorMessage = "Invalid e-mail or password!";
        private const int OneWeekInSeconds = 604800;

        public LoginTests(TestingWebApplicationFactory factory)
        {
            this.factory = factory;

            this.client = this.factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                }).CreateClient();
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "admin")]
        public async Task LoginUserSuccessfully(string email, string password, string username)
        {
            var user = new LoginUserInputModel
            {
                Email = email,
                Password = password,
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(LoginEndpoint, json);
            var content = JsonConvert.DeserializeObject<LoginViewModel>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(content.Token);
            var claims = decodedToken.Claims;

            Assert.Equal(username, claims.First(c => c.Type == "unique_name").Value);
            Assert.Equal(email, claims.First(c => c.Type == "email").Value);

            Assert.False(Convert.ToBoolean(claims.First(c => c.Type == "isBanned").Value));
            Assert.True(Convert.ToBoolean(claims.First(c => c.Type == "isAdmin").Value));

            Assert.Equal(Convert.ToInt32(claims.First(c => c.Type == "iat").Value) + OneWeekInSeconds, Convert.ToInt32(claims.First(c => c.Type == "exp").Value));
        }

        [Theory]
        [InlineData("test@test.com", "12341234")]
        public async Task LoginFailWithInvalidEmail(string email, string password)
        {
            var user = new LoginUserInputModel
            {
                Email = email,
                Password = password,
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(LoginEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(LoginErrorMessage, content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12345678")]
        public async Task LoginFailWithInvalidPassword(string email, string password)
        {
            var user = new LoginUserInputModel
            {
                Email = email,
                Password = password,
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(LoginEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(LoginErrorMessage, content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }
    }
}
