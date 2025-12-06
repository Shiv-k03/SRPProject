using SRP.Repository.Entities;

namespace SRP.Repository.Interfaces
{
    public interface IMarkRepository : IRepository<Mark>
    {
        Task<Mark?> GetMarkWithDetailsAsync(int markId);
        Task<IEnumerable<Mark>> GetMarksByStudentAsync(int studentId);
        Task<IEnumerable<Mark>> GetMarksBySubjectAsync(int subjectId);
        Task<IEnumerable<Mark>> GetMarksByStudentAndSubjectAsync(int studentId, int subjectId);
        Task<Mark?> GetMarkByStudentSubjectAndExamAsync(int studentId, int subjectId, string examType);
        Task<Mark> AddOrUpdateMarkAsync(Mark mark);
    }
}
