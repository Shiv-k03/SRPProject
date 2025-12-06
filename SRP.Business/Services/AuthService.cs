using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SRP.Business.Interfaces;
using SRP.Model.DTOs.Requests;
using SRP.Repository.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;

namespace SRP.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<ResultModel> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(request.Username);

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                {
                    return ResultModel.Unauthorized("Invalid username or password");
                }

                if (!user.IsActive)
                {
                    return ResultModel.Unauthorized("User account is inactive");
                }

                user.LastLogin = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                var token = GenerateJwtToken(user.UserId, user.Username, user.Role.ToString());

                var response = new
                {
                    Token = token,
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(Constants.JwtExpirationMinutes)
                };

                return ResultModel.Success(response, "Login successful");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error during login: {ex.Message}");
            }
        }

        public async Task<ResultModel> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return ResultModel.NotFound("User not found");
                }

                // Verify current password
                if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                {
                    return ResultModel.Invalid("Current password is incorrect");
                }

                // Validate new password is different from current
                if (request.CurrentPassword == request.NewPassword)
                {
                    return ResultModel.Invalid("New password must be different from current password");
                }

                // Update password
                user.PasswordHash = HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                return ResultModel.Success(null, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error changing password: {ex.Message}");
            }
        }

        public string GenerateJwtToken(int userId, string username, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Constants.JwtExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == passwordHash;
        }
    }
}