using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.Models.Users;

namespace Forum.Data.DataTransferObjects
{
    public class MapInitialization : Profile
    {
        public MapInitialization()
        {
            this.CreateMap<RegisterUserInputModel, User>();
        }
    }
}
