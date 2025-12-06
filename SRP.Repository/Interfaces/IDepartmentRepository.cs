using SRP.Repository.Entities;

namespace SRP.Repository.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department?> GetDepartmentWithDetailsAsync(int departmentId);
        Task<IEnumerable<Department>> GetAllDepartmentsWithDetailsAsync();
        Task<bool> IsDepartmentCodeExistsAsync(string departmentCode);
        Task<Department?> GetByCodeAsync(string departmentCode);
        Task<Department> AddOrUpdateDepartmentAsync(Department department);
    }
}
