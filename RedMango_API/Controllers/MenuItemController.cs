using EcomApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;
using RedMango_API.Models.Dto;

using RedMango_API.Utility;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace RedMango_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMenuRepository menuRepository;
        private ApiResponse _response;
    

        public MenuItemController( IMenuRepository menuRepository)
        {
          
            
            this.menuRepository = menuRepository;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
          
            _response.Result= await menuRepository.GetMenuItems();
            return Ok(_response);
        }

        [HttpGet("{id:int}",Name = "GetMenuItem")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            
            if (id == 0)
            {
                return new BadRequestObjectResult(new { StatusCode = HttpStatusCode.BadRequest, IsSuccess = false, Message = "Invalid ID" });
            }

            MenuItem menuItem=await menuRepository.GetMenuItem(id);
            if (menuItem == null)
            {
                return new NotFoundObjectResult(new { StatusCode = HttpStatusCode.NotFound, IsSuccess = false, Message = "Menu item not found" });
            }

            return new OkObjectResult(menuItem);



        }

        
    }
}
