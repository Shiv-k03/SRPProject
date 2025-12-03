using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace StudentReportPortal.Infrastructure.Repositories
{
    public class StudentSubjectRepository : Repository<StudentSubject>, IStudentSubjectRepository
    {
        public StudentSubjectRepository(ApplicationDbContext context, IMapper mapper) 
            : base(context, mapper)
        {
        }

        public async Task<StudentSubject?> GetStudentSubjectWithDetailsAsync(int studentSubjectId)
        {
            return await _dbSet
                .Include(ss => ss.Student)
                    .ThenInclude(s => s.User)
                .Include(ss => ss.Subject)
                .FirstOrDefaultAsync(ss => ss.StudentSubjectId == studentSubjectId && !ss.IsDeleted);
        }

        public async Task<IEnumerable<StudentSubject>> GetSubjectsByStudentAsync(int studentId)
        {
            return await _dbSet
                .Include(ss => ss.Subject)
                    .ThenInclude(s => s.Department)
                .Include(ss => ss.Student)
                .Where(ss => ss.StudentId == studentId && !ss.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentSubject>> GetStudentsBySubjectAsync(int subjectId)
        {
            return await _dbSet
                .Include(ss => ss.Student)
                    .ThenInclude(s => s.User)
                .Include(ss => ss.Subject)
                .Where(ss => ss.SubjectId == subjectId && !ss.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsStudentEnrolledInSubjectAsync(int studentId, int subjectId, int semester)
        {
            return await _dbSet.AnyAsync(ss =>
                ss.StudentId == studentId &&
                ss.SubjectId == subjectId &&
                ss.Semester == semester &&
                !ss.IsDeleted);
        }
    }
}
