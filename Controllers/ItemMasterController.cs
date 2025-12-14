using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesOrderManagement.BusinessLogic.Interfaces;

namespace SalesOrderManagement.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemMasterController : ControllerBase
    {
        private readonly IItemMasterService _service;

        public ItemMasterController(IItemMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] int? itemId)
        {
            var items = await _service.GetItemsAsync(itemId);
            return Ok(items);
        }
    }
}
