using System.Threading.Tasks;
using SalesOrderManagement.Models.DTOs;

namespace SalesOrderManagement.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<UserDetailsDto?> GetUserByIdAsync(int userId);
    }
}
