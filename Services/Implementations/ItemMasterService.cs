using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesOrderManagement.BusinessLogic.Interfaces;
using SalesOrderManagement.DataAccess;
using SalesOrderManagement.Models.Entities;

namespace SalesOrderManagement.Services.Implementations
{
    public class ItemMasterService : IItemMasterService
    {
        private readonly SalesOrderDbContext _context;

        public ItemMasterService(SalesOrderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemMaster>> GetItemsAsync(int? itemId)
        {
            var query = _context.ItemMasters.AsQueryable();

            if (itemId.HasValue)
            {
                query = query.Where(i => i.ItemId == itemId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
