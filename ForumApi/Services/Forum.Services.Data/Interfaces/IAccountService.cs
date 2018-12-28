using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.User;

namespace Forum.Services.Data.Interfaces
{
    public interface IAccountService
    {
        Task Register(RegisterUserInputModel model);

        Task SeedAdmin(RegisterUserInputModel model);

        Task<string> Login(LoginUserInputModel model);

        ICollection<LoginInfoViewModel> GetUserLoginInfo(string username);
    }
}
