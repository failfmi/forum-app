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
using Microsoft.EntityFrameworkCore;
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
                throw new ArgumentException("The provided post for this comment is not valid.");
            }

            var user = this.userRepository.Query().FirstOrDefault(u => u.UserName == username);

            var comment = this.Mapper.Map<CommentInputModel, Comment>(model);
            comment.Author = user;
            comment.CreationDate = DateTime.UtcNow;

            await this.commentRepository.AddAsync(comment);
            await this.commentRepository.SaveChangesAsync();

            return this.Mapper.Map<Comment, CommentViewModel>(comment);
        }

        public async Task<CommentViewModel> Edit(CommentInputEditModel model, string username)
        {
            if (IsValidComment(model.Id))
            {
                throw new ArgumentException($"Comment with id '{model.Id}' does not exist.");
            }

            var user = this.userRepository.Query().FirstOrDefault(u => u.UserName == username);

            var comment = this.commentRepository.Query().Include(c => c.Author).First(c => c.Id == model.Id);

            var isCallerAdmin = await this.UserManager.IsInRoleAsync(user, "Admin");
            if (comment?.Author.Email != user?.Email && !isCallerAdmin)
            {
                throw new UnauthorizedAccessException("You are not allowed for this operation.");
            }

            comment.Text = model.Text;
            comment.CreationDate = DateTime.UtcNow;

            this.commentRepository.Update(comment);
            await this.commentRepository.SaveChangesAsync();

            return this.Mapper.Map<CommentViewModel>(comment);
        }

        public async Task Delete(int id, string username)
        {
            if (this.IsValidComment(id))
            {
                throw new Exception($"Comment with id '{id}' does not exist.");
            }

            var user = this.userRepository.Query().FirstOrDefault(u => u.UserName == username);
            var isCallerAdmin = await this.UserManager.IsInRoleAsync(user, "Admin");

            var comment = this.commentRepository
                .Query()
                .Include(c => c.Author)
                .First(c => c.Id == id);

            if (comment.Author.Email != user?.Email && !isCallerAdmin)
            {
                throw new UnauthorizedAccessException("You are not allowed for this operation.");
            }

            this.commentRepository.Delete(comment);
            await this.commentRepository.SaveChangesAsync();
        }

        public ICollection<CommentViewModel> GetCommentsByUsername(string username)
        {
            var comments = this.commentRepository.Query().Where(c => c.Author.UserName == username).ToList();

            return this.Mapper.Map<ICollection<CommentViewModel>>(comments);
        }

        private bool IsValidComment(int id)
        {
            return this.commentRepository.Query().Count(c => c.Id == id) == 0;
        }

        private Post GetPost(int postId)
        {
            return this.postRepository.Query().AsNoTracking().FirstOrDefault(p => p.Id == postId);
        }
    }
}
