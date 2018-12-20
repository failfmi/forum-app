using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.Category;
using Forum.Data.Models;
using Forum.Data.Models.Users;

namespace Forum.Data.DataTransferObjects
{
    public class MapInitialization : Profile
    {
        public MapInitialization()
        {
            this.CreateMap<CategoryInputModel, Category>();
            this.CreateMap<CategoryInputEditModel, Category>();
            this.CreateMap<Category, CategoryViewModel>();

            this.CreateMap<RegisterUserInputModel, User>();
        }
    }
}
