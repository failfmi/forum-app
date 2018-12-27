using System;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.ContactUs;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Forum.WebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/contact-us")]
    public class ContactUsController : BaseController
    {
        private readonly IContactUsService contactUsService;
        public ContactUsController(IContactUsService contactUsService, ILogger<BaseController> logger) : base(logger)
        {
            this.contactUsService = contactUsService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> ContactUs([FromBody] ContactUsInputModel model)
        {
            try
            {
                await this.contactUsService.Create(model);

                return this.Ok(new ReturnMessage() { Message = "Thank you for reaching us. We will contact you soon." });
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }
    }
}
