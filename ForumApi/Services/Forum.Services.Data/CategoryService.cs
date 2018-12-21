using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.Category;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly IRepository<Category> categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository, UserManager<User> userManager, ILogger<BaseService> logger, IMapper mapper) : base(userManager, logger, mapper)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<CategoryViewModel> Create(CategoryInputModel model)
        {
            if (!IsValidCategoryName(model.Name))
            {
                throw new Exception($"Category with name '{model.Name}' already exists.");
            }

            var category = this.Mapper.Map<CategoryInputModel, Category>(model);

            await this.categoryRepository.AddAsync(category);
            await this.categoryRepository.SaveChangesAsync();

            return this.Mapper.Map<Category, CategoryViewModel>(category);
        }

        public async Task<CategoryViewModel> Edit(CategoryInputEditModel model)
        {
            if (IsValidCategoryId(model.Id))
            {
                throw new Exception($"Category with id {model.Id} does not exist.");
            }

            if (!IsValidCategoryName(model.Name))
            {
                throw new Exception($"Category with name '{model.Name}' already exists.");
            }

            var category = this.Mapper.Map<CategoryInputEditModel, Category>(model);

            this.categoryRepository.Update(category);
            await this.categoryRepository.SaveChangesAsync();

            return this.Mapper.Map<Category, CategoryViewModel>(category);
        }

        public Task Delete(int id)
        {
            //if (IsValidCategoryId(id))
            //{
            //    throw new Exception($"Category with id {id} does not exist.");
            //}

            //this.categoryRepository.Delete(id);
            throw new NotImplementedException();
        }

        public CategoryViewModel GetById(int id)
        {
            if (IsValidCategoryId(id))
            {
                throw new Exception($"Category with id {id} does not exist.");
            }

            var category = this.categoryRepository.Query().FirstOrDefault(c => c.Id == id);

            return this.Mapper.Map<CategoryViewModel>(category);
        }

        public ICollection<CategoryViewModel> All()
        {
            var categories = this.categoryRepository.Query().ToList();

            return this.Mapper.Map<ICollection<CategoryViewModel>>(categories);
        }

        private bool IsValidCategoryId(int id)
        {
            return this.categoryRepository.Query().Count(c => c.Id == id) == 0;
        }

        private bool IsValidCategoryName(string name)
        {
            return this.categoryRepository.Query().Count(c => c.Name == name) == 0;
        }
    }
}
