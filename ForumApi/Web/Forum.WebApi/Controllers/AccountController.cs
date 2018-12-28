using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.User;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Forum.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService, ILogger<BaseController> logger)
            : base(logger)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> Register([FromBody] RegisterUserInputModel model)
        {
            try
            {
                await this.accountService.Register(model);

                return this.Ok(new ReturnMessage() { Message = "You have successfully registered! Now you should be able to log in!" });
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> Login([FromBody] LoginUserInputModel model)
        {
            try
            {
                var token = await this.accountService.Login(model);
                return this.Ok(new LoginViewModel {Message = "You have successfully logged in!", Token = token});
            }
            catch (UnauthorizedAccessException e)
            {
                return this.Unauthorized(new ReturnMessage { Message = e.Message});
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage {Message = "Invalid e-mail or password!"});
            }
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> History()
        {
            try
            {
                var history = this.accountService.GetUserLoginInfo(this.User.FindFirst(ClaimTypes.Name).Value);
                return this.Ok(history);
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = "Something went wrong!" });
            }
        }
    }
}
