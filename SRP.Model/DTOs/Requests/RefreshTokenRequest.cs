
namespace SRP.Model.DTOs.Requests
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string JwtToken { get; set; } = string.Empty;
    }
}
