using System.Collections.Generic;
using System.Threading.Tasks;
using SalesOrderManagement.Models.Entities;

namespace SalesOrderManagement.BusinessLogic.Interfaces
{
    public interface IItemMasterService
    {
        Task<IEnumerable<ItemMaster>> GetItemsAsync(int? itemId);
    }
}
