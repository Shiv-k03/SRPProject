using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Model.Helper.Base;

namespace SRP.Business.Interfaces
{
    public interface IMarkService
    {
        Task<ResultModel> AddOrUpdateMarkAsync(MarkRequest request);
        Task<ResultModel> GetMarkByIdAsync(int markId);
        Task<ResultModel> GetMarkByFilterAsync(MarkFilterModel filter);
        Task<ResultModel> DeleteMarkAsync(int markId);
    }
}