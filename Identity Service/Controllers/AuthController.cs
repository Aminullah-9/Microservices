using Identity_Service.DTO;
using Identity_Service.Models;
using Identity_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterDTO registerDTO)
        {
            var existinguser = await _userManager.FindByEmailAsync(registerDTO.Email);

            if(existinguser != null)
            {
                return BadRequest("User Already existed");
            }

            var user = new ApplicationUser
            {
                UserName=registerDTO.UserName,
                Email=registerDTO.Email
            };
            var result= await _userManager.CreateAsync(user,registerDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }


            return Ok(new
            {
                Message = "User registered successfully"
            });
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

            var tokken = _tokenService.GenerateTokken(user);

            return Ok(new
            {
                Token = tokken
            });
        }

    }
}
