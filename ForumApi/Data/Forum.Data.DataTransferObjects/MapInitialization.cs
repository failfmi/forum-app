using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.InputModels.Comment;
using Forum.Data.DataTransferObjects.InputModels.ContactUs;
using Forum.Data.DataTransferObjects.InputModels.Post;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.Comment;
using Forum.Data.DataTransferObjects.ViewModels.Post;
using Forum.Data.DataTransferObjects.ViewModels.User;
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

            this.CreateMap<CommentInputModel, Comment>();
            this.CreateMap<CommentInputEditModel, Comment>();
            this.CreateMap<Comment, CommentViewModel>()
                .ForMember(cvm => cvm.Author, conf => conf.MapFrom(c => c.Author.UserName));

            this.CreateMap<ContactUsInputModel, ContactUs>();

            this.CreateMap<PostInputModel, Post>();
            this.CreateMap<PostInputEditModel, Post>();
            this.CreateMap<Post, PostViewModel>()
                .ForMember(pvm => pvm.Author, conf => conf.MapFrom(p => p.Author.UserName));

            this.CreateMap<RegisterUserInputModel, User>();

            this.CreateMap<User, UserViewModel>()
                .ForMember(uvm => uvm.IsBanned, conf => conf.MapFrom(u => !u.IsActive));
        }
    }
}
