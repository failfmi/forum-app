using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Data.Services.Tests.Fake;
using Forum.Services.Data;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace Forum.Data.Services.Tests
{
    public class CategoryServiceTests
    {
        private readonly ICategoryService categoryService;
        private readonly IRepository<Category> categoryRepository;
        private readonly ForumContext context;

        public CategoryServiceTests()
        {
            var guid = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase(guid).Options;
            this.context = new ForumContext(options);
            this.categoryRepository = new Repository<Category>(context);

            var userStore = new UserStore<User>(context);
            var logger = new Mock<ILogger<CategoryService>>();

            var mapperProfile = new MapInitialization();
            var conf = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            var mapper = new Mapper(conf);

            var fakeUserManager = new FakeUserManager(userStore);

            this.categoryService = new CategoryService(categoryRepository, fakeUserManager, logger.Object, mapper);
        }

        [Fact]
        public async Task ShouldReturnEmptyCollection()
        {
            var all = this.categoryService.All();
            Assert.Empty(all);
        }

        [Theory]
        [InlineData(1, "Programming")]
        public async Task ShouldFailEditInvalidId(int id, string categoryName)
        {
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await this.categoryService.Edit(new CategoryInputEditModel { Id = id, Name = categoryName }));

            Assert.Equal($"Category with id {id} does not exist.", exception.Message);
        }

        [Theory]
        [InlineData(1)]
        public async Task ShouldFailGetCategoryById(int categoryId)
        {
            var exception = Assert.Throws<Exception>(() => this.categoryService.GetById(categoryId));
            Assert.Equal($"Category with id {categoryId} does not exist.", exception.Message);
        }

        [Theory]
        [InlineData("Programming")]
        public async Task ShouldAddCategory(string categoryName)
        {
            var category = await this.categoryService.Create(new CategoryInputModel { Name = categoryName });
            var totalCategories = this.categoryService.All();

            Assert.Equal(category.Id, category.Id);
            Assert.Equal(categoryName, category.Name);
            Assert.Equal(1, totalCategories.Count);
        }

        [Theory]
        [InlineData("Test", "Test1234")]
        public async Task ShouldEditSuccessfully(string categoryName, string newName)
        {
            var category = new Category
            {
                Name = categoryName
            };

            await this.categoryRepository.AddAsync(category);
            await this.categoryRepository.SaveChangesAsync();
            this.context.Entry(category).State = EntityState.Detached;

            var categoryToEdit = this.categoryRepository.Query().AsNoTracking().Last();

            var edit = await this.categoryService.Edit(new CategoryInputEditModel
            { Id = categoryToEdit.Id, Name = newName });

            var totalCategories = this.categoryService.All();
            Assert.Equal(newName, totalCategories.Last().Name);
        }

        [Theory]
        [InlineData("Programming")]
        public async Task ShouldFailAddCategorySecondTimeSameName(string categoryName)
        {
            await this.categoryService.Create(new CategoryInputModel { Name = categoryName });
            var exception =
                await Assert.ThrowsAsync<Exception>(async () => await this.categoryService.Create(new CategoryInputModel { Name = categoryName }));

            Assert.Equal($"Category with name '{categoryName}' already exists.", exception.Message);
        }

        [Theory]
        [InlineData("Test")]
        public async Task ShouldFailEditInvalidName(string categoryName)
        {
            var category = await this.categoryService.Create(new CategoryInputModel { Name = categoryName });

            var exception = await Assert.ThrowsAsync<Exception>(async () =>
                await this.categoryService.Edit(new CategoryInputEditModel { Id = category.Id, Name = categoryName }));
            Assert.Equal($"Category with name '{categoryName}' already exists.", exception.Message);
        }

        [Theory]
        [InlineData("Programming")]
        public async Task ShouldGetSuccessfully(string categoryName)
        {
            var categoryView = await this.categoryService.Create(new CategoryInputModel { Name = categoryName });
            var category = this.categoryService.GetById(categoryView.Id);

            Assert.Equal(categoryName, category.Name);
        }

        [Theory]
        [InlineData(42)]
        public async Task ShouldDeleteFailInvalidId(int id)
        {
            var exception =
                await Assert.ThrowsAsync<Exception>(async () => await this.categoryService.Delete(id));

            Assert.Equal($"Category with id {id} does not exist.", exception.Message);
        }

        [Theory]
        [InlineData("Programming")]
        public async Task ShouldDeleteSuccessfully(string name)
        {
            var asdf = this.categoryService.All();
            Assert.Equal(0, asdf.Count);
            var cat = await this.categoryService.Create(new CategoryInputModel { Name = name });
            await this.categoryService.Delete(cat.Id);
            var all = this.categoryService.All();
            Assert.Equal(0, all.Count);

            var exception =
                await Assert.ThrowsAsync<Exception>(async () => await this.categoryService.Delete(cat.Id));

            Assert.Equal($"Category with id {cat.Id} does not exist.", exception.Message);
        }
    }
}
