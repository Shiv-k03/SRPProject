using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Base;

namespace SRP.Business.Interfaces
{
    public interface IUserService
    {
        Task<ResultModel> GetUserByIdAsync(int userId);
        Task<ResultModel> GetUserProfileAsync(int userId);
        Task<ResultModel> GetUserByFilterAsync(UserFilterModel filter);
        Task<ResultModel> UpdateUserAsync(int userId, UpdateUserRequest request);
        Task<ResultModel> DeactivateUserAsync(int userId);
        Task<ResultModel> ActivateUserAsync(int userId);

        //Task<ResultModel> CreateAdminAsync(CreateAdminRequest request);
    }
}