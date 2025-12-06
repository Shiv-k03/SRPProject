using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace SRP.Repository.Repositories
{
    public class TeacherSubjectRepository : Repository<TeacherSubject>, ITeacherSubjectRepository
    {
        public TeacherSubjectRepository(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<TeacherSubject?> GetTeacherSubjectWithDetailsAsync(int teacherSubjectId)
        {
            return await _dbSet
                .Include(ts => ts.Teacher)
                    .ThenInclude(t => t.User)
                .Include(ts => ts.Subject)
                .FirstOrDefaultAsync(ts => ts.TeacherSubjectId == teacherSubjectId && !ts.IsDeleted);
        }

        public async Task<IEnumerable<TeacherSubject>> GetSubjectsByTeacherAsync(int teacherId)
        {
            return await _dbSet
                .Include(ts => ts.Subject)
                    .ThenInclude(s => s.Department)
                .Include(ts => ts.Teacher)
                .Where(ts => ts.TeacherId == teacherId && !ts.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeacherSubject>> GetTeachersBySubjectAsync(int subjectId)
        {
            return await _dbSet
                .Include(ts => ts.Teacher)
                    .ThenInclude(t => t.User)
                .Include(ts => ts.Subject)
                .Where(ts => ts.SubjectId == subjectId && !ts.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsTeacherAssignedToSubjectAsync(int teacherId, int subjectId, int semester)
        {
            return await _dbSet.AnyAsync(ts =>
                ts.TeacherId == teacherId &&
                ts.SubjectId == subjectId &&
                ts.Semester == semester &&
                !ts.IsDeleted);
        }
    }
}