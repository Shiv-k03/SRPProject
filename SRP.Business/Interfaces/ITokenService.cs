using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(int userId, string username, string role);
        LoginResponse GenerateTokenResponse(int userId, string username, string email, string fullName, string role);
    }
}
