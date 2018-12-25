using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Post;
using Forum.Data.DataTransferObjects.ViewModels;
using Forum.Data.DataTransferObjects.ViewModels.Post;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Forum.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class PostController : BaseController
    {
        private readonly IPostService postService;

        public PostController(IPostService postService, ILogger<BaseController> logger) : base(logger)
        {
            this.postService = postService;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<object> Create([FromBody] PostInputModel model)
        {
            try
            {
                var post = await this.postService.Create(model, this.User.FindFirst(ClaimTypes.Email).Value);

                return this.Ok(new CreateEditReturnMessage<PostViewModel>
                { Message = "Post created successfully", Data = post });
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<object> Edit(int id, [FromBody] PostInputEditModel model)
        {
            if (id != model.Id)
            {
                return this.BadRequest(new ReturnMessage { Message = "Invalid ids" });
            }

            try
            {
                var post = await this.postService.Edit(model, this.User.FindFirst(ClaimTypes.Email).Value);
                return this.Ok(new CreateEditReturnMessage<PostViewModel>
                { Message = "Post edited successfully", Data = post });
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Unauthorized(new ReturnMessage { Message = e.Message });
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> Delete(int id)
        {
            try
            {
                await this.postService.Delete(id, this.User.FindFirst(ClaimTypes.Email).Value);
                return this.Ok(new ReturnMessage { Message = "Post deleted successfully!" });
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Unauthorized(new ReturnMessage { Message = e.Message });
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> Get(int id)
        {
            try
            {
                return this.Ok(this.postService.GetPostById(id));
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> All()
        {
            try
            {
                var categories = this.postService.All();
                return this.Ok(categories);
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }
    }
}
