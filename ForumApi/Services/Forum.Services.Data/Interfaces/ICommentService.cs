using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Comment;
using Forum.Data.DataTransferObjects.ViewModels.Comment;

namespace Forum.Services.Data.Interfaces
{
    public interface ICommentService
    {
        Task<CommentViewModel> Create(CommentInputModel model, string username);

        Task<CommentViewModel> Edit(CommentInputEditModel model, string username);

        Task Delete(int id, string username);

        ICollection<CommentViewModel> GetCommentsByUsername(string username);
    }
}
