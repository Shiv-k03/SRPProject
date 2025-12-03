
using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentResponse> CreateDepartmentAsync(DepartmentRequest request, string createdBy);
        Task<DepartmentResponse> GetDepartmentByIdAsync(int departmentId);
        Task<IEnumerable<DepartmentResponse>> GetAllDepartmentsAsync();
        Task<DepartmentResponse> UpdateDepartmentAsync(int departmentId, DepartmentRequest request, string updatedBy);
        Task DeleteDepartmentAsync(int departmentId);
    }
}