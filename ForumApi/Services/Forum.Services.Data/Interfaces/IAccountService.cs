using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.User;

namespace Forum.Services.Data.Interfaces
{
    public interface IAccountService
    {
        Task<bool> Register(RegisterUserInputModel model);

        Task SeedAdmin(RegisterUserInputModel model);
    }
}
