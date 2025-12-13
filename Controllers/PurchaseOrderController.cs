using System;
using System.Threading.Tasks;
using SalesOrderManagement.BusinessLogic.Interfaces;
using SalesOrderManagement.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SalesOrderManagement.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _service;

        public PurchaseOrderController(IPurchaseOrderService service)
        {
            _service = service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var result = await _service.ProcessPurchaseOrderAsync(file);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PurchaseOrderApiResponseDto 
                { 
                    SourceFileName = file.FileName,
                    Success = false, 
                    ErrorMessage = ex.Message 
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePurchaseOrderRequestDto request)
        {
            if (request == null)
                return BadRequest("Invalid request body.");

            try
            {
                var result = await _service.CreatePurchaseOrderAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                 return StatusCode(500, new PurchaseOrderApiResponseDto 
                { 
                    SourceFileName = "Manual Entry",
                    Success = false, 
                    ErrorMessage = ex.Message 
                });
            }
        }
    }
}
