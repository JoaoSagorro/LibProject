using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ADOLib;
using static ADOLib.Model.Model;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController()
        {
            _userService = new UserService();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest("Invalid user data.");

            try
            {
                user.RoleId = 3;

                if (_userService.EmailExists(user.Email))
                    return Conflict(new { error = "Email is already in use." });

                bool isRegistered = await _userService.RegisterUser(user);
                if (isRegistered)
                    return Ok(new { message = "User registered successfully" });
                else
                    return BadRequest("Registration failed. User may already exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
