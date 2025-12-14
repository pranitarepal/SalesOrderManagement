using System;
using System.IO;
using System.Linq;
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

            // Validate file extension
            var allowedExtensions = new[] { ".xlsx", ".xls", ".docx", ".doc", ".txt", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new PurchaseOrderApiResponseDto
                {
                    SourceFileName = file.FileName,
                    DetectedType = fileExtension ?? "Unknown",
                    Success = false,
                    ErrorMessage = $"Invalid file format. Only Excel (.xlsx, .xls), Word (.docx, .doc), Text (.txt), and PDF files are allowed. Uploaded file: {fileExtension ?? "no extension"}"
                });
            }

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

        [HttpPost("create")]
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
