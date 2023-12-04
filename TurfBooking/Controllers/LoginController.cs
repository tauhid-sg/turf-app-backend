using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TurfBooking.Data;
using TurfBooking.Models;

namespace TurfBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly PlayerRepository _playerRepository;

        public LoginController(PlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            bool isValid = await _playerRepository.ValidatePlayerLogin(model.PlayerName, model.PlayerPassword);

            if (isValid)
            {
                // Successful login
                return Ok(new { message = "Login successful" });
            }
            else
            {
                // Invalid credentials
                return BadRequest(new { message = "Invalid credentials" });
            }
        }
    }
}
