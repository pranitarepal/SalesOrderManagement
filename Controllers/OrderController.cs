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
    }
}
