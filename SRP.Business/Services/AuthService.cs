using Microsoft.Extensions.Configuration;
using SRP.Business.Interfaces;
using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Base;
using SRP.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SRP.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public async Task<ResultModel> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(request.Username);
                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                    return ResultModel.Unauthorized("Invalid username or password");

                if (!user.IsActive)
                    return ResultModel.Unauthorized("User account is inactive");

                user.LastLogin = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                var tokenResponse = _tokenService.GenerateTokenResponse(
                    user.UserId,
                    user.Username,
                    user.Email,
                    $"{user.FirstName} {user.LastName}",
                    user.Role.ToString());

                return ResultModel.Success(tokenResponse, "Login successful");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception("Error during login", ex.Message);
            }
        }

        public async Task<ResultModel> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || user.IsDeleted)
                    return ResultModel.NotFound("User not found");

                if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                    return ResultModel.Invalid("Current password is incorrect");

                if (request.CurrentPassword == request.NewPassword)
                    return ResultModel.Invalid("New password must be different from current password");

                user.PasswordHash = HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                return ResultModel.Success(null, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception("Error changing password", ex.Message);
            }
        }

        public Task<ResultModel> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var result = ResultModel.Unauthorized("Refresh token functionality not implemented yet");
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var errorResult = ResultModel.Exception("Error during token refresh", ex.Message);
                return Task.FromResult(errorResult);
            }
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == passwordHash;
        }
    }
}
