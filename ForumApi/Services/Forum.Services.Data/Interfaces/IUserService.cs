using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.ViewModels.User;

namespace Forum.Services.Data.Interfaces
{
    public interface IUserService
    {
        Task<UserViewModel> Ban(string id);

        Task<UserViewModel> UnBan(string id);

        Task<ICollection<UserViewModel>> All();
    }
}
