using Identity_Service.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity_Service.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public string GenerateTokken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id
                ),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email!
                ),

                new Claim(
                    JwtRegisteredClaimNames.UniqueName,
                    user.UserName!
                )
            };


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
    }
}