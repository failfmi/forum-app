using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.InputModels.Comment;
using Forum.Data.DataTransferObjects.ViewModels.Comment;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public class CommentService : BaseService, ICommentService
    {
        private readonly IRepository<Comment> commentRepository;
        private readonly IRepository<Post> postRepository;
        private readonly IRepository<User> userRepository;

        public CommentService(IRepository<Comment> commentRepository, IRepository<User> userRepository, IRepository<Post> postRepository, UserManager<User> userManager, ILogger<BaseService> logger, IMapper mapper) : base(userManager, logger, mapper)
        {
            this.userRepository = userRepository;
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
        }

        public async Task<CommentViewModel> Create(CommentInputModel model, string username)
        {
            var post = this.GetPost(model.PostId);
            if (post is null)
            {
                throw new ArgumentException("Post for this comment does not exist");
            }

            var user = this.userRepository.Query().FirstOrDefault(u => u.UserName == username);

            var comment = this.Mapper.Map<CommentInputModel, Comment>(model);
            comment.Author = user;
            comment.Post = post;
            comment.CreationDate = DateTime.UtcNow;

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();

            return this.Mapper.Map<Comment, CommentViewModel>(comment);
        }

        public Task<CommentViewModel> Edit(CommentViewModel model)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public ICollection<CommentViewModel> GetCommentsByUsername(string username)
        {
            var comments = this.commentRepository.Query().Where(c => c.Author.UserName == username).ToList();

            return this.Mapper.Map<ICollection<CommentViewModel>>(comments);
        }

        private Post GetPost(int postId)
        {
            return this.postRepository.Query().FirstOrDefault(p => p.Id == postId);
        }
    }
}
