using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.Category;
using Forum.Data.DataTransferObjects.ViewModels;
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<object> Get(int id)
        {
            try
            {
                return this.Ok(this.categoryService.GetById(id));
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
                var categories = this.categoryService.All();
                return this.Ok(categories);
            }
            catch (Exception e)
            {
                return this.BadRequest(new ReturnMessage { Message = e.Message });
            }
        }
    }
}
