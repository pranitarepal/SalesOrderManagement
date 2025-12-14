using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesOrderManagement.BusinessLogic.Interfaces;
using SalesOrderManagement.DataAccess;
using SalesOrderManagement.Models.DTOs;

namespace SalesOrderManagement.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly SalesOrderDbContext _context;

        public UserService(SalesOrderDbContext context)
        {
            _context = context;
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return null;
                }

                return new UserDetailsDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    ContactNo = user.ContactNo ?? string.Empty,
                    Address = user.Address ?? string.Empty,
                    ZipCode = user.ZipCode ?? string.Empty,
                    UserType = user.UserType ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user details: {ex.Message}", ex);
            }
        }
    }
}
