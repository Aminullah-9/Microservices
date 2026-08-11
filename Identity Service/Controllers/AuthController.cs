using Identity_Service.Data;
using Identity_Service.DTO;
using Identity_Service.Models;
using Identity_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, TokenService tokenService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterDTO registerDTO)
        {
            var existinguser = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (existinguser != null)
            {
                return BadRequest("User Already existed");
            }

            var user = new ApplicationUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            return Ok("User Registered Successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return BadRequest("Invalid Credinatiials");
            }

            var validpassword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!validpassword)
            {
                return BadRequest("Invalid Credinatiials");
            }

            var accessToken =
        await _tokenService.GenerateTokken(user);

            var refreshToken =
                 _tokenService.GenerateRefreshToken(user);

            // IMPORTANT
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefereshToken(RefreshTokenDTO refreshTokenDTO)
        {
            var refereshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshTokenDTO.RefereshToken);

            if (refereshToken == null)
            {
                return BadRequest("Invalid Refresh Token");
            }
            if (refereshToken.IsRevoked)
            {
                return BadRequest("The tokken Is Revoked");
            }
            if (refereshToken.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("The tokken Is Expired");
            }
            var user = await _userManager.FindByIdAsync( refereshToken.UserId);

            if (user == null)
            {
                return BadRequest(
                    "User associated with token does not exist.");
            }
            refereshToken.IsRevoked = true;

            // Generate new tokens
            var newAccessToken = await _tokenService.GenerateTokken(user);

            var newRefreshToken =
                 _tokenService.GenerateRefreshToken(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });

        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutDTO logoutDTO)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == logoutDTO.RefreshToken);

            if (refreshToken == null)
            {
                return BadRequest("Invalid Refresh Token");
            }

            if (refreshToken.IsRevoked)
            {
                return BadRequest("Refresh Token is already revoked");
            }

            refreshToken.IsRevoked = true;

            await _context.SaveChangesAsync();

            return Ok("Logged out successfully");
        }
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User identity not found.");
            }

            var refreshTokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync();

            if (!refreshTokens.Any())
            {
                return Ok("No active sessions found.");
            }

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();

            return Ok("All sessions have been logged out.");
        }
    }
}
