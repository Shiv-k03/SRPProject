using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<string> GenerateJwtTokenAsync(int userId, string username, string role);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}