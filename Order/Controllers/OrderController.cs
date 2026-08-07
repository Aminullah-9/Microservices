using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.DTO;

namespace Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {

            var response = await _orderService.GetOrders();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var response = await _orderService.GetOrderById(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
        {
            var response = await _orderService.CreateOrder(order);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return StatusCode(201, response);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(OrderUpdateDTO order)
        {
            var response = await _orderService.UpdateOrder(order);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var response = await _orderService.DeleteOrder(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}