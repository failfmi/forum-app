using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.Category;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests
{
    public class CategoryControllerTests : BaseControllerTest
    {
        private const string CategoryCreateEndpoint = "api/admin/category/create";
        private const string CategoryEditEndpoint = "api/admin/category/edit/";
        private const string CategoryDeleteEndpoint = "api/admin/category/delete/";
        private const string CategoryGetByIdEndpoint = "api/category/get/";
        private const string CategoryAllEndpoint = "api/category/all";

        private const string UnauthorizedError = "You are unauthorized!";

        public CategoryControllerTests(TestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", "Baseball")]
        public async Task CreateCategorySuccessfully(string email, string password, string categoryName)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputModel
            {
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(CategoryCreateEndpoint, json);

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

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputModel
            {
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(CategoryCreateEndpoint, json);

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

            var response = await this.client.PostAsync(CategoryCreateEndpoint, json);

            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test", "Baseball")]
        public async Task CreateCategoryFailDueToUnauthorizedToken(string email, string password, string username, string categoryName)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputModel
            {
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PostAsync(CategoryCreateEndpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
            Assert.Equal(UnauthorizedError, content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1, "Baseball")]
        public async Task EditCategorySuccessfully(string email, string password, int id, string categoryName)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputEditModel
            {
                Id = id,
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PutAsync(CategoryEditEndpoint + id, json);

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

            var response = await this.client.PutAsync(CategoryEditEndpoint + id, json);

            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test", 1, "Education2")]
        [InlineData("test@test.com", "12341234", "test", 2, "Test")]
        public async Task EditCategoryFailDueToUnauthorizedToken(string email, string password, string username, int id, string categoryName)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var category = new CategoryInputEditModel
            {
                Id = id,
                Name = categoryName
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(category),
                Encoding.UTF8,
                "application/json");

            var response = await this.client.PutAsync(CategoryEditEndpoint + id, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
            Assert.Equal(UnauthorizedError, content.Message);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1, "Blockchain")]
        public async Task EditCategoryFailDueToAlreadyExistingCategory(string email, string password, int id,
            string categoryName)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

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

            var response = await this.client.PutAsync(endpoint, json);

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

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

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

            var response = await this.client.PutAsync(endpoint, json);

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

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

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

            var response = await this.client.PutAsync(endpoint, json);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal("Invalid ids", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("test@test.com", "12341234", "test", 1)]
        [InlineData("test@test.com", "12341234", "test", 2)]
        public async Task DeleteCategoryFailDueToUnauthorizedToken(string email, string password, string username, int id)
        {
            await this.Register(email, password, username);
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.DeleteAsync(CategoryDeleteEndpoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status401Unauthorized, content.Status);
            Assert.Equal(UnauthorizedError, content.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task DeleteCategoryFailDueToUnauthorized(int id)
        {
            var response = await this.client.DeleteAsync(CategoryDeleteEndpoint + id);

            Assert.Equal(StatusCodes.Status401Unauthorized, (int)response.StatusCode);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 8)]
        [InlineData("admin@admin.com", "12341234", 42)]
        public async Task DeleteCategoryFailDueToInvalidId(string email, string password, int id)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.DeleteAsync(CategoryDeleteEndpoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Category with id {id} does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
        }

        [Theory]
        [InlineData("admin@admin.com", "12341234", 1)]
        public async Task DeleteCategorySuccessfully(string email, string password, int id)
        {
            var token = await this.Login(email, password);

            this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await this.client.DeleteAsync(CategoryDeleteEndpoint + id);

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            Assert.Equal($"Category with id {id} successfully deleted", content.Message);
        }

        [Theory]
        [InlineData(1, "Education")]
        public async Task GetCategoryByIdSuccessfully(int categoryId, string name)
        {
            var response = await this.client.GetAsync(CategoryGetByIdEndpoint + categoryId);
            var content = JsonConvert.DeserializeObject<CategoryViewModel>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            Assert.Equal(name, content.Name);
            Assert.Equal(categoryId, content.Id);
        }

        [Theory]
        [InlineData(42)]
        public async Task GetCategoryByIdFail(int categoryId)
        {
            var response = await this.client.GetAsync(CategoryGetByIdEndpoint + categoryId);
            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal($"Category with id {categoryId} does not exist.", content.Message);
            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);

        }

        [Fact]
        public async Task GetAllSuccessfully()
        {
            string[] expected =
                {"Education", "Football", "Basketball", "Marketing", "Blockchain", "Programming", "Game Theory"};
            var response = await this.client.GetAsync(CategoryAllEndpoint);
            var content = JsonConvert.DeserializeObject<IEnumerable<CategoryViewModel>>(await response.Content.ReadAsStringAsync());

            response.EnsureSuccessStatusCode();

            int i = 0;
            foreach (var categoryViewModel in content)
            {
                Assert.Equal(expected[i], categoryViewModel.Name);
                i++;
            }
        }
    }
}
