using System.Threading.Tasks;

namespace SalesOrderManagement.BusinessLogic.Interfaces
{
    public interface IOrderService
    {
        Task<bool> DeleteOrderAsync(int id);
        Task<bool> CancelOrderAsync(int id);
    }
}
