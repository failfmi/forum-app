using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Post;
using Forum.Data.DataTransferObjects.ViewModels.Post;
using Forum.Data.Models.Users;

namespace Forum.Services.Data.Interfaces
{
    public interface IPostService
    {
        Task<PostViewModel> Create(PostInputModel model, string email);

        Task<PostViewModel> Edit(PostInputEditModel model, string email);

        Task Delete(int id, string email);

        ICollection<PostViewModel> All();

        PostViewModel GetPostById(int id);
    }
}
