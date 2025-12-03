using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace StudentReportPortal.Infrastructure.Repositories
{
    public class MarkRepository : Repository<Mark>, IMarkRepository
    {
        public MarkRepository(ApplicationDbContext context, IMapper mapper) 
            : base(context, mapper)
        {

        }

        public async Task<Mark?> GetMarkWithDetailsAsync(int markId)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.MarkId == markId && !m.IsDeleted);
        }

        public async Task<IEnumerable<Mark>> GetMarksByStudentAsync(int studentId)
        {
            return await _dbSet
                .Include(m => m.Subject)
                .Where(m => m.StudentId == studentId && !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mark>> GetMarksBySubjectAsync(int subjectId)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Where(m => m.SubjectId == subjectId && !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mark>> GetMarksByStudentAndSubjectAsync(int studentId, int subjectId)
        {
            return await _dbSet
                .Include(m => m.Subject)
                .Where(m => m.StudentId == studentId && m.SubjectId == subjectId && !m.IsDeleted)
                .ToListAsync();
        }

        public async Task<Mark?> GetMarkByStudentSubjectAndExamAsync(int studentId, int subjectId, string examType)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m =>
                    m.StudentId == studentId &&
                    m.SubjectId == subjectId &&
                    m.ExamType == examType &&
                    !m.IsDeleted);
        }
    }
}
