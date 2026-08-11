using Identity_Service.Data;
using Identity_Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity_Service.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration; 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ApplicationDbContext context) 
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> GenerateTokken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.Id),
               new Claim(JwtRegisteredClaimNames.Email, user.Email!),
               new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );


            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );


            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],

                audience: _configuration["Jwt:Audience"],

                claims: claims,

                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(
                        _configuration["Jwt:DurationInMinutes"]
                    )
                ),

                signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(ApplicationUser user)
        {
            var randombytes = RandomNumberGenerator.GetBytes(64);

            var RefereToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randombytes),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                UserId = user.Id
            };

            _context.RefreshTokens.Add(RefereToken);
            return RefereToken;
        }
    }
}