using SRP.Repository.Entities;

namespace SRP.Repository.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
        Task<Teacher?> GetTeacherWithDetailsAsync(int teacherId);
        Task<Teacher?> GetTeacherByUserIdAsync(int userId);
        Task<Teacher?> GetTeacherByEmployeeCodeAsync(string employeeCode);
        Task<IEnumerable<Teacher>> GetTeachersByDepartmentAsync(int departmentId);
        Task<bool> IsEmployeeCodeExistsAsync(string employeeCode);
    }
}
