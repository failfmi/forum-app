using System;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Controllers;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Forum.WebApi.Areas.Admin.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class AdminController : BaseController
    {
        private readonly IUserService userService;
        public AdminController(IUserService userService, ILogger<BaseController> logger) : base(logger)
        {
            this.userService = userService;
        }

        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<object> Ban(string id)
        {
            if (this.User.IsInRole("Admin"))
            {
                try
                {
                    var user = await this.userService.Ban(id);

                    return this.Ok(new CreateEditReturnMessage<UserViewModel>
                    { Message = "User banned successfully", Data = user });
                }
                catch (Exception e)
                {
                    return this.BadRequest(new ReturnMessage { Message = e.Message });
                }
            }

            return this.Unauthorized(new ReturnMessage { Message = "You are unauthorized!" });
        }

        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<object> UnBan(string id)
        {
            if (this.User.IsInRole("Admin"))
            {
                try
                {
                    var user = await this.userService.UnBan(id);

                    return this.Ok(new CreateEditReturnMessage<UserViewModel>
                    { Message = "User unbanned successfully", Data = user });
                }
                catch (Exception e)
                {
                    return this.BadRequest(new ReturnMessage { Message = e.Message });
                }
            }

            return this.Unauthorized(new ReturnMessage { Message = "You are unauthorized!" });
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> All()
        {
            if (this.User.IsInRole("Admin"))
            {
                try
                {
                    var users = await this.userService.All();
                    return this.Ok(users);
                }
                catch (Exception e)
                {
                    return this.BadRequest(new ReturnMessage { Message = e.Message });
                }
            }

            return this.Unauthorized(new ReturnMessage { Message = "You are unauthorized!" });
        }
    }
}
