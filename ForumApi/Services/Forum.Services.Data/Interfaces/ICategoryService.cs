﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.Category;

namespace Forum.Services.Data.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> Create(CategoryInputModel model);

        Task<CategoryViewModel> Edit(CategoryInputEditModel model);

        Task Delete(int id);
    }
}