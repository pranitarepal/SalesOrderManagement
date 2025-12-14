using System.Threading.Tasks;

namespace SalesOrderManagement.BusinessLogic.Interfaces
{
    public interface IOrderService
    {
        Task<bool> DeleteOrderAsync(int id);
        Task<bool> CancelOrderAsync(int id);
        Task<SalesOrderManagement.Models.DTOs.OrderDetailsDto?> GetOrderDetailsAsync(int orderId);
    }
}
