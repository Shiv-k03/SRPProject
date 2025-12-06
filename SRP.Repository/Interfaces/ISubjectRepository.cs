using SRP.Repository.Entities;

namespace SRP.Repository.Interfaces
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject?> GetSubjectWithDetailsAsync(int subjectId);
        Task<Subject?> GetSubjectByCodeAsync(string subjectCode);
        Task<IEnumerable<Subject>> GetSubjectsByDepartmentAsync(int departmentId);
        Task<IEnumerable<Subject>> GetSubjectsBySemesterAsync(int semester);
        Task<bool> IsSubjectCodeExistsAsync(string subjectCode);
        Task<Subject> AddOrUpdateSubjectAsync(Subject subject);
    }
}
