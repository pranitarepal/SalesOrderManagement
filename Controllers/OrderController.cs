using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesOrderManagement.BusinessLogic.Interfaces;

namespace SalesOrderManagement.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _service.DeleteOrderAsync(id);
            if (!result)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok($"Order {id} status updated to Deleted (Soft Delete).");
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _service.CancelOrderAsync(id);
            if (!result)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok($"Order {id} status updated to Cancelled.");
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails([FromQuery] int? orderId, [FromQuery] string? userId)
        {
            // If orderId is provided, fetch specific order
            if (orderId.HasValue)
            {
                var result = await _service.GetOrderDetailsAsync(orderId.Value);
                if (result == null)
                {
                    return NotFound($"Order with ID {orderId} not found.");
                }
                return Ok(result);
            }

            // If userId is provided, fetch all orders for that user
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var result = await _service.GetOrdersByUserIdAsync(userId);
                return Ok(result);
            }

            // If neither is provided, return bad request
            return BadRequest("Either orderId or userId must be provided.");
        }
    }
}
