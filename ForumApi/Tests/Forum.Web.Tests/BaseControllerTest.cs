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
        private const string LoginEndpoint = "api/account/login";
        private const string RegisterEndpoint = "api/account/register";

        protected readonly TestingWebApplicationFactory factory;
        protected readonly HttpClient client;
        protected const string ValidationMessage = "One or more validation errors occurred.";

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

        public async Task Register(string email, string password, string username)
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

            response.EnsureSuccessStatusCode();
        }
    }
}
