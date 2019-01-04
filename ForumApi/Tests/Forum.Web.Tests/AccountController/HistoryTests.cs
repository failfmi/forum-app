using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests.AccountController
{
    public class HistoryTests : BaseControllerTest
    {
        private const string HistoryEndpoint = "api/account/history";
        private const string HistoryErrorMessage = "Something went wrong!";

        public HistoryTests(TestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "207.97.227.239")]
        public async Task GetUserLoginHistorySuccessfully(string email, string password, string expectedIp)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.GetAsync(HistoryEndpoint);

            response.EnsureSuccessStatusCode();

            var content = JsonConvert.DeserializeObject<ICollection<LoginInfoViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Single(content);
            Assert.Equal(expectedIp, content.First().Ip);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "207.97.227.239")]
        public async Task GetUserLoginHistoryFailDueToNoToken(string email, string password, string expectedIp)
        {
            var response = await this.client.GetAsync(HistoryEndpoint);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());
            
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal(HistoryErrorMessage, content.Message);
        }
    }
}
