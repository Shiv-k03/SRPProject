using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SRP.Business.Interfaces;
using SRP.Model.DTOs.Responses;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;
using SRP.Repository.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace SRP.Business.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;

            var secretKey = _configuration.GetSection("JwtSettings")["SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string GenerateJwtToken(int userId, string username, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role, role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Constants.JwtExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public LoginResponse GenerateTokenResponse(int userId, string username, string email, string fullName, string role)
        {
            var token = GenerateJwtToken(userId, username, role);

            return new LoginResponse
            {
                Token = token,
                UserId = userId,
                Username = username,
                Email = email,
                FullName = fullName,
                Role = Enum.Parse<SRP.Repository.Enums.UserRole>(role),
                ExpiresAt = DateTime.UtcNow.AddMinutes(Constants.JwtExpirationMinutes)
            };
        }
    }
}
