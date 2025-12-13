using System.Threading.Tasks;
using SalesOrderManagement.BusinessLogic.Interfaces;
using SalesOrderManagement.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace SalesOrderManagement.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly SalesOrderDbContext _context;

        public OrderService(SalesOrderDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return false;
            }

            // Soft delete: User requested "status off by 0", interpreted as IsDeleted = true
            order.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return false;
            }

            order.OrderStatus = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
