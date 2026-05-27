using API_CHAT.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShareModel;

namespace API_CHAT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;
        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var exists = await _context.Users.FirstOrDefaultAsync(u => (u.Username == request.Username || u.Email == request.Username) && u.PasswordHash == request.Password);
            if (exists != null)
            {
                _logger.LogInformation("User {Username} logged in successfully. UID: {UId}", exists.Username, exists.UId);
                return Ok(new { Message = "Login successful", uid = exists.UId, username = exists.Username, email = exists.Email});
            }
            else
            {
                _logger.LogInformation("Login failed");
                return Unauthorized(new { Message = "Invalid username or password" });
            }
        }
    }
}
