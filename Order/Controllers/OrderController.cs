using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.DTO;
using Order.Services.Interfaces;

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

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders();
            return Ok(orders);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(OrderUpdateDTO order)
        {
            var updatedOrder = await _orderService.UpdateOrder(order);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return Ok(updatedOrder);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
        {
            var createdOrder = await _orderService.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrders), new { id = createdOrder.OrderId }, createdOrder);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
    }
}
