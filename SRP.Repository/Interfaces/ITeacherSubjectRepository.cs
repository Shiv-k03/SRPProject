
using SRP.Repository.Entities;

namespace SRP.Repository.Interfaces
{
    public interface ITeacherSubjectRepository : IRepository<TeacherSubject>
    {
        Task<TeacherSubject?> GetTeacherSubjectWithDetailsAsync(int teacherSubjectId);
        Task<IEnumerable<TeacherSubject>> GetSubjectsByTeacherAsync(int teacherId);
        Task<IEnumerable<TeacherSubject>> GetTeachersBySubjectAsync(int subjectId);
        Task<bool> IsTeacherAssignedToSubjectAsync(int teacherId, int subjectId, int semester);
    }
}