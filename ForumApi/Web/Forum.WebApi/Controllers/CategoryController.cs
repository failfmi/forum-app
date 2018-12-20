using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.Category;
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
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService, ILogger<BaseController> logger) : base(logger)
        {
            this.categoryService = categoryService;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<object> Create([FromBody] CategoryInputModel model)
        {
            if (this.User.IsInRole("Admin"))
            {
                try
                {
                    var category = await this.categoryService.Create(model);

                    return this.Ok(new CategorySuccessReturnMessage
                    { Message = "Category added successfully", Data = category });
                }
                catch (Exception e)
                {
                    return this.BadRequest(new ReturnMessage { Message = e.Message });
                }
            }

            return this.Unauthorized(new ReturnMessage {Message = "You are unauthorized!"});
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<object> Edit(int id, [FromBody] CategoryInputEditModel model)
        {
            if (id != model.Id)
            {
                return this.BadRequest(new ReturnMessage { Message = "Invalid ids" });
            }

            if (this.User.IsInRole("Admin"))
            {
                try
                {
                    var category = await this.categoryService.Edit(model);

                    return this.Ok(new CategorySuccessReturnMessage
                    { Message = "Category edited successfully", Data = category });
                }
                catch (Exception e)
                {
                    return this.BadRequest(new ReturnMessage { Message = e.Message });
                }
            }

            return this.Unauthorized();
        }
    }
}
