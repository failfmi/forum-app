using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Comment;
using Forum.Data.DataTransferObjects.ViewModels;
using Forum.Data.DataTransferObjects.ViewModels.Comment;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Forum.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class CommentController : BaseController
    {
        private ICommentService commentService;
        public CommentController(ICommentService commentService, ILogger<BaseController> logger) : base(logger)
        {
            this.commentService = commentService;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Create([FromBody] CommentInputModel model)
        {
            try
            {
                var comment = await this.commentService.Create(model, this.User.FindFirst(ClaimTypes.Name).Value);

                return this.Ok(new CreateEditReturnMessage<CommentViewModel>
                    {Message = "Comment created successfully!", Data = comment});
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage {Message = e.Message});
            }
        }
        
    }
}
