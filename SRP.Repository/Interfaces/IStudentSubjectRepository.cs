using SRP.Repository.Entities;

namespace SRP.Repository.Interfaces
{
    public interface IStudentSubjectRepository : IRepository<StudentSubject>
    {
        Task<StudentSubject?> GetStudentSubjectWithDetailsAsync(int studentSubjectId);
        Task<IEnumerable<StudentSubject>> GetSubjectsByStudentAsync(int studentId);
        Task<IEnumerable<StudentSubject>> GetStudentsBySubjectAsync(int subjectId);
        Task<bool> IsStudentEnrolledInSubjectAsync(int studentId, int subjectId, int semester);
    }
}
