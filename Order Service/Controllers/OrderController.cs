using Common.OrderData.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Interfaces;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IOrderService _orderService; 
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "Order service is running." });
        }

        // POST api/orders
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("with-users")]
        //[Authorize]
        public async Task<IActionResult> GetOrdersWithUsers()
        {
            try
            {
                var ordersWithUsers = await _orderService.GetOrdersWithUserInfoAsync();
                return Ok(ordersWithUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
