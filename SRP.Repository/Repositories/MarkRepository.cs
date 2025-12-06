using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace SRP.Repository.Repositories
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

        public async Task<Mark> AddOrUpdateMarkAsync(Mark mark)
        {
            // Check if updating by MarkId
            if (mark.MarkId > 0)
            {
                var existingMark = await _dbSet
                    .FirstOrDefaultAsync(m => m.MarkId == mark.MarkId && !m.IsDeleted);

                if (existingMark != null)
                {
                    // Update existing mark
                    existingMark.StudentId = mark.StudentId;
                    existingMark.SubjectId = mark.SubjectId;
                    existingMark.TeacherId = mark.TeacherId;
                    existingMark.ObtainedMarks = mark.ObtainedMarks;
                    existingMark.TotalMarks = mark.TotalMarks;
                    existingMark.ExamType = mark.ExamType;
                    existingMark.ExamDate = mark.ExamDate;
                    existingMark.Remarks = mark.Remarks;
                    existingMark.Grade = mark.Grade;
                    existingMark.UpdatedBy = mark.UpdatedBy;
                    existingMark.UpdatedAt = DateTime.UtcNow;

                    _dbSet.Update(existingMark);
                    await _context.SaveChangesAsync();
                    return existingMark;
                }
            }

            // Add new mark
            mark.MarkId = 0; // Ensure it's a new entity
            mark.CreatedAt = DateTime.UtcNow;
            mark.UpdatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(mark);
            await _context.SaveChangesAsync();
            return mark;
        }
    }
}
