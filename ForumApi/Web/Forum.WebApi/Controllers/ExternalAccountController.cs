using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.ViewModels.ExternalAuth;
using Forum.Data.DataTransferObjects.ViewModels.User;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Forum.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/external/[action]")]
    public class ExternalAccountController : BaseController
    {
        private readonly IExternalAccountService externalAccountService;

        public ExternalAccountController(IExternalAccountService externalAccountService, ILogger<BaseController> logger) : base(logger)
        {
            this.externalAccountService = externalAccountService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Facebook([FromBody] FacebookLoginModel model)
        {
            try
            {
                var token = await this.externalAccountService.FacebookLogin(model);
                return this.Ok(new LoginViewModel { Message = "You have successfully logged in!", Token = token });
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }
    }
}
