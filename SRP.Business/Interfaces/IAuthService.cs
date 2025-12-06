using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Base;

namespace SRP.Business.Interfaces
{
    public interface IAuthService
    {
        Task<ResultModel> LoginAsync(LoginRequest request);
        Task<ResultModel> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        string GenerateJwtToken(int userId, string username, string role);
    }
}