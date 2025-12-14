using System;
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

        public async Task<OrderDetailsDto?> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);


                if (order == null)
                {
                    return null;
                }

                return new OrderDetailsDto
                {
                    OrderId = order.OrderId,
                    OrderStatus = order.OrderStatus ?? "Pending",
                    OrderDate = order.CreatedDate,
                    Notes = order.Notes,
                    ItemDetails = order.OrderItems?.Select(i => new OrderItemDto
                    {
                        ItemId = i.ItemId,
                        ProductName = i.ProductName ?? string.Empty,
                        Manufacturer = i.Manufacturer ?? string.Empty,
                        Price = i.Price ?? 0,
                        Qty = i.Qty ?? 0,
                        Subtotal = i.Subtotal ?? 0
                    }).ToList() ?? new List<OrderItemDto>()
                };
            }
            catch (Exception ex)
            {
                // Log the actual error for debugging
                throw new Exception($"Error fetching order details: {ex.Message}", ex);
            }
        }

        public async Task<OrderListDto> GetOrdersByUserIdAsync(string userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .AsNoTracking()
                    .Where(o => o.UserId == userId && o.IsDeleted != true)
                    .ToListAsync();

                var orderList = new OrderListDto
                {
                    Orders = orders.Select(order => new OrderDetailsDto
                    {
                        OrderId = order.OrderId,
                        OrderStatus = order.OrderStatus ?? "Pending",
                        OrderDate = order.CreatedDate,
                        Notes = order.Notes,
                        ItemDetails = order.OrderItems?.Select(i => new OrderItemDto
                        {
                            ItemId = i.ItemId,
                            ProductName = i.ProductName ?? string.Empty,
                            Manufacturer = i.Manufacturer ?? string.Empty,
                            Price = i.Price ?? 0,
                            Qty = i.Qty ?? 0,
                            Subtotal = i.Subtotal ?? 0
                        }).ToList() ?? new List<OrderItemDto>()
                    }).ToList()
                };

                return orderList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching orders for user {userId}: {ex.Message}", ex);
            }
        }
    }
}
