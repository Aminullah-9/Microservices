using Identity_Service.Data;
using Identity_Service.DTO;
using Identity_Service.Models;
using Identity_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var accesstokken = await _tokenService.GenerateTokken(user);
            var RefereshToken = await _tokenService.GenerateRefreshToken(user);
            return Ok(new
            {
                Token = accesstokken,
                RefreshToken = RefereshToken.Token
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

            var user = await _userManager.FindByIdAsync(refereshToken.UserId);
            if (user == null)
            {
                return BadRequest("User associated with token does not exist.");
            }
            var newAccessToken = await _tokenService.GenerateTokken(user);
            return Ok(new
            {
                Token = newAccessToken,
            });

        }
    }
}
