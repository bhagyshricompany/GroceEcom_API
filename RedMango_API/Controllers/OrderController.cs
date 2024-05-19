using EcomApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedMango_API.Models.Dto;
using RedMango_API.Models;
using System.Net;
using System.Text.Json;

namespace EcomApi.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApiResponse _response;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetOrders(string? userId, string searchString, string status, int pageNumber = 1, int pageSize = 5)
        {
            var orderHeaders = await _orderRepository.GetOrdersAsync(userId, searchString, status, pageNumber, pageSize);

            Pagination pagination = new()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalRecords = orderHeaders.Count(),
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

            _response.Result = orderHeaders;
            return Ok(_response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse>> GetOrder(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var orderHeader = await _orderRepository.GetOrderByIdAsync(id);
            if (orderHeader == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.Result = orderHeader;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
        {
            var order = await _orderRepository.CreateOrderAsync(orderHeaderDTO);
            _response.Result = order;
            _response.StatusCode = HttpStatusCode.Created;
            return Ok(_response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrderHeader(int id, [FromBody] OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            if (orderHeaderUpdateDTO == null || id != orderHeaderUpdateDTO.OrderHeaderId)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var result = await _orderRepository.UpdateOrderHeaderAsync(id, orderHeaderUpdateDTO);
            if (!result)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }
}