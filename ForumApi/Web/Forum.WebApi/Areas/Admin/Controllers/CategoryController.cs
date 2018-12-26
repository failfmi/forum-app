using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.ViewModels.Category;
using Forum.Services.Data.Interfaces;
using Forum.WebApi.Controllers;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Forum.WebApi.Areas.Admin.Controllers
{
    [Route("api/admin/[controller]/[action]")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService categoryService, ILogger<BaseController> logger) : base(logger)
        {
            this.categoryService = categoryService;
        }

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

                    return this.Ok(new CreateEditReturnMessage<CategoryViewModel>
                    { Message = "Category created successfully", Data = category });
                }
                catch (Exception e)
                {
                    return this.BadRequest(new ReturnMessage { Message = e.Message });
                }
            }

            return this.Unauthorized(new ReturnMessage { Message = "You are unauthorized!" });
        }

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

                    return this.Ok(new CreateEditReturnMessage<CategoryViewModel>
                    { Message = "Category edited successfully", Data = category });
                }
                catch (Exception e)
                {
                    return this.BadRequest(new ReturnMessage { Message = e.Message });
                }
            }

            return this.Unauthorized(new ReturnMessage { Message = "You are unauthorized!" });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public async Task<object> Delete(int id)
        {
            if (this.User.IsInRole("Admin"))
            {
                try
                {
                    await this.categoryService.Delete(id);
                    return this.Ok(new ReturnMessage { Message = $"Category with id {id} successfully deleted" });
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
