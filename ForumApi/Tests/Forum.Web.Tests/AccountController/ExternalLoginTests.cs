using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.ViewModels.ExternalAuth;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests.AccountController
{
    public class ExternalLoginTests : IClassFixture<TestingWebApplicationFactory>
    {
        private readonly TestingWebApplicationFactory factory;
        private readonly HttpClient client;

        private const string LoginFacebookEndpoint = "api/external/facebook";
        private const string LoginInvalidFacebookTokenMessage = "Invalid Facebook Token.";

        public ExternalLoginTests(TestingWebApplicationFactory factory)
        {
            this.factory = factory;

            this.client = this.factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                }).CreateClient();
        }

        [Theory]
        [InlineData("EAAEMkfF5gCoBAIrZBrkB8klZBMerLJ1l1rCd2QMT9WhZCK3aC1lXZB3DwtoAIufkk8sPmXoXhr0KIPlqMlRYTeKi6PoyhkF7iZBVZBy9S9Wzv6s9yZBayRZCbCLBqSxsyzPOLVddxEyZBp3SZBJEWGWGi9YSwodYbg95mU2ZCxcuTaOSlgFQJbvq9816aTlCmZBAC4qvgsq8c6p8hAjBRYiMkYeR")]
        public async Task LoginFacebookFailWithInvalidToken(string token)
        {
            var user = new FacebookLoginModel
            {
                Token = token
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(user),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(LoginFacebookEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(LoginInvalidFacebookTokenMessage, content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }
    }
}
