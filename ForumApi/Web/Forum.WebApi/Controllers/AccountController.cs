using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.User;
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

                return this.Ok(new ReturnMessage() { Message = "Registration Successful." });
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage() { Message = e.Message });
            }
        }
    }
}
