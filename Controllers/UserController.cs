using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesOrderManagement.BusinessLogic.Interfaces;

namespace SalesOrderManagement.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get customer details by User ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details or 404 if not found</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id==0)
            {
                return BadRequest("User ID is not correct.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound($"User with ID '{id}' not found.");
            }

            return Ok(user);
        }
    }
}
