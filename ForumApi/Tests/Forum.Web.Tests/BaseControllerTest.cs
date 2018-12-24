using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests
{
    public abstract class BaseControllerTest : IClassFixture<TestingWebApplicationFactory>
    {
        protected readonly TestingWebApplicationFactory factory;
        protected readonly HttpClient client;

        private const string LoginEndpoint = "api/account/login";

        protected BaseControllerTest(TestingWebApplicationFactory factory)
        {
            this.factory = factory;

            this.client = this.factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                }).CreateClient();
        }

        protected async Task<string> Login(string email, string password)
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

            var response = await this.client.PostAsync(LoginEndpoint, json);
            var content = JsonConvert.DeserializeObject<LoginViewModel>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            return content.Token;
        }
    }
}
