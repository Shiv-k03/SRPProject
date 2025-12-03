using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<UserProfileResponseDto> GetUserProfileAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserRequest request, string updatedBy);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task DeactivateUserAsync(int userId);
        Task ActivateUserAsync(int userId);
    }
}