using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Model.Helper.Base;

namespace SRP.Business.Interfaces
{
    public interface IDepartmentService
    {
        Task<ResultModel> AddOrUpdateDepartmentAsync(DepartmentRequest request);
        Task<ResultModel> GetDepartmentByIdAsync(int departmentId);
        Task<ResultModel> GetDepartmentByFilterAsync(DepartmentFilterModel filter);
        Task<ResultModel> DeleteDepartmentAsync(int departmentId);
    }
}