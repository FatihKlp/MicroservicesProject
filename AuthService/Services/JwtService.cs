using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuthService.Models;
using AuthService.Interfaces;

namespace AuthService.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public JwtService(IConfiguration config)
        {
            // Environment variables'dan oku, yoksa appsettings'ten
            _jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
                ?? config["Jwt:Key"]
                ?? throw new ArgumentException("JWT Key is not configured");

            _jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? config["Jwt:Issuer"]
                ?? throw new ArgumentException("JWT Issuer is not configured");

            _jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                ?? config["Jwt:Audience"]
                ?? "AuthServiceUsers";
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}