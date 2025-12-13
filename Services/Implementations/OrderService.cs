using System.Threading.Tasks;
using SalesOrderManagement.BusinessLogic.Interfaces;
using SalesOrderManagement.DataAccess;
using Microsoft.EntityFrameworkCore;

using SalesOrderManagement.Models.DTOs;
using System.Linq;

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

        public async Task<OrderDetailsDto> GetOrderDetailsAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            return new OrderDetailsDto
            {
                OrderInfo = new OrderInfoDto
                {
                    OrderId = order.OrderId,
                    OrderStatus = order.OrderStatus
                },
                ItemDetails = order.OrderItems.Select(i => new OrderItemDto
                {
                    ItemId = i.ItemId,
                    ProductName = i.ProductName,
                    Manufacturer = i.Manufacturer,
                    Price = i.Price,
                    Qty = i.Qty,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }
    }
}
