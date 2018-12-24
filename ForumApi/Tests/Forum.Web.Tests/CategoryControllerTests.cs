using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels;
using Forum.Data.DataTransferObjects.ViewModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests
{
    public class CategoryControllerTests : IClassFixture<TestingWebApplicationFactory>
    {
        private readonly TestingWebApplicationFactory factory;
        private readonly HttpClient client;

        private const string LoginEndpoint = "api/account/login";
        private const string CategoryCreateEndpoint = "api/category/create";
        private const string CategoryEditEndpoint = "api/category/edit/";
        private const string CategoryDeletePoint = "api/category/delete/";
        private const string CategoryGetByIdEndpoint = "api/category/get/";
        private const string CategoryAllEndpoint = "api/category/all";

        public CategoryControllerTests(TestingWebApplicationFactory factory)
        {
            this.factory = factory;

            this.client = this.factory
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                }).CreateClient();
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "Baseball")]
        public async Task CreateCategorySuccessfully(string email, string password, string categoryName)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputModel
            {
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(CategoryCreateEndpoint, json);

            response.EnsureSuccessStatusCode();

            var content = JsonConvert.DeserializeObject<CreateEditReturnMessage<CategoryViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal("Category created successfully", content.Message);
            Assert.Equal(StatusCodes.Status200OK, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "Blockchain")]
        public async Task CreateCategoryFailDueToAlreadyExistingCategory(string email, string password,
            string categoryName)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputModel
            {
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(CategoryCreateEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Category with name '{categoryName}' already exists.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("Baseball")]
        public async Task CreateCategoryFailDueToUnauthorized(string categoryName)
        {
            var category = new CategoryInputModel
            {
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(CategoryCreateEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1, "Baseball")]
        public async Task EditCategorySuccessfully(string email, string password, int id, string categoryName)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputEditModel
            {
                Id = id,
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync(CategoryEditEndpoint + id, json);

            response.EnsureSuccessStatusCode();

            var content = JsonConvert.DeserializeObject<CreateEditReturnMessage<CategoryViewModel>>(await response.Content.ReadAsStringAsync());

            Assert.Equal("Category edited successfully", content.Message);
            Assert.Equal(StatusCodes.Status200OK, content.Status);
        }

        [Theory]
        [InlineData(1, "Education2")]
        [InlineData(2, "Test")]
        public async Task EditCategoryFailDueToUnauthorized(int id, string categoryName)
        {
            var category = new CategoryInputEditModel
            {
                Id = id,
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync(CategoryEditEndpoint + id, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1, "Blockchain")]
        public async Task EditCategoryFailDueToAlreadyExistingCategory(string email, string password, int id,
            string categoryName)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputEditModel
            {
                Id = id,
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var endpoint = CategoryEditEndpoint + id;

            var response = await client.PutAsync(endpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Category with name '{categoryName}' already exists.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 8, "Blockchain")]
        public async Task EditCategoryFailDueToInvalidId(string email, string password, int id,
            string categoryName)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputEditModel
            {
                Id = id,
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var endpoint = CategoryEditEndpoint + id;

            var response = await client.PutAsync(endpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Category with id {id} does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 2, 1, "Blockchain")]
        public async Task EditCategoryFailDueToInvalidIdsInEndpoint(string email, string password, int id, int invalidId,
            string categoryName)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputEditModel
            {
                Id = id,
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var endpoint = CategoryEditEndpoint + invalidId;

            var response = await client.PutAsync(endpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal("Invalid ids", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task DeleteCategoryFailDueToUnauthorized(int id)
        {
            var response = await client.DeleteAsync(CategoryDeletePoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 8)]
        [InlineData("admin@admin.com", "12341234", 42)]
        public async Task DeleteCategoryFailDueToInvalidId(string email, string password, int id)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.DeleteAsync(CategoryDeletePoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Category with id {id} does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1)]
        public async Task DeleteCategorySuccessfully(string email, string password, int id)
        {
            var token = await this.Login(email, password);

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.DeleteAsync(CategoryDeletePoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            Assert.Equal($"Category with id {id} successfully deleted", content.Message);
        }

        [Theory]
        [InlineData(1, "Education")]
        public async Task GetCategoryByIdSuccessfully(int categoryId, string name)
        {
            var response = await client.GetAsync(CategoryGetByIdEndpoint + categoryId);
            var content = JsonConvert.DeserializeObject<CategoryViewModel>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            Assert.Equal(name,content.Name);
            Assert.Equal(categoryId, content.Id);
        }

        [Theory]
        [InlineData(42)]
        public async Task GetCategoryByIdFail(int categoryId)
        {
            var response = await client.GetAsync(CategoryGetByIdEndpoint + categoryId);
            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Category with id {categoryId} does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            
        }

        [Fact]
        public async Task GetAllSuccessfully()
        {
            string[] expected =
                {"Education", "Football", "Basketball", "Marketing", "Blockchain", "Programming", "Game Theory"};
            var response = await client.GetAsync(CategoryAllEndpoint);
            var content = JsonConvert.DeserializeObject<IEnumerable<CategoryViewModel>>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            int i = 0;
            foreach (var categoryViewModel in content)
            {
                Assert.Equal(expected[i], categoryViewModel.Name);
                i++;
            }
        }

        private async Task<string> Login(string email, string password)
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

            return content.Token;
        }
    }
}
