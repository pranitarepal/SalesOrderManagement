using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SalesOrderManagement.Models.DTOs;

namespace SalesOrderManagement.BusinessLogic.Interfaces
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrderApiResponseDto> ProcessPurchaseOrderAsync(IFormFile file);
        Task<PurchaseOrderApiResponseDto> CreatePurchaseOrderAsync(CreatePurchaseOrderRequestDto request);
    }
}
