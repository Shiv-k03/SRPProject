using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace SRP.Repository.Repositories
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<Subject?> GetSubjectWithDetailsAsync(int subjectId)
        {
            return await _dbSet
                .Include(s => s.Department)
                .Include(s => s.TeacherSubjects)
                    .ThenInclude(ts => ts.Teacher)
                        .ThenInclude(t => t.User)
                .Include(s => s.StudentSubjects)
                    .ThenInclude(ss => ss.Student)
                        .ThenInclude(st => st.User)
                .FirstOrDefaultAsync(s => s.SubjectId == subjectId && !s.IsDeleted);
        }

        public async Task<Subject?> GetSubjectByCodeAsync(string subjectCode)
        {
            return await _dbSet
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.SubjectCode == subjectCode && !s.IsDeleted);
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Include(s => s.Department)
                .Where(s => s.DepartmentId == departmentId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetSubjectsBySemesterAsync(int semester)
        {
            return await _dbSet
                .Include(s => s.Department)
                .Where(s => s.Semester == semester && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsSubjectCodeExistsAsync(string subjectCode)
        {
            return await _dbSet.AnyAsync(s => s.SubjectCode == subjectCode && !s.IsDeleted);
        }

        public async Task<Subject> AddOrUpdateSubjectAsync(Subject subject)
        {
            // Check if updating by SubjectId
            if (subject.SubjectId > 0)
            {
                var existingSubject = await _dbSet
                    .FirstOrDefaultAsync(s => s.SubjectId == subject.SubjectId && !s.IsDeleted);

                if (existingSubject != null)
                {
                    // Update existing subject
                    existingSubject.SubjectName = subject.SubjectName;
                    existingSubject.SubjectCode = subject.SubjectCode;
                    existingSubject.Credits = subject.Credits;
                    existingSubject.Semester = subject.Semester;
                    existingSubject.DepartmentId = subject.DepartmentId;
                    existingSubject.Description = subject.Description;
                    existingSubject.MaxMarks = subject.MaxMarks;
                    existingSubject.PassingMarks = subject.PassingMarks;
                    existingSubject.UpdatedBy = subject.UpdatedBy;
                    existingSubject.UpdatedAt = DateTime.UtcNow;

                    _dbSet.Update(existingSubject);
                    await _context.SaveChangesAsync();
                    return existingSubject;
                }
            }

            // Add new subject
            subject.SubjectId = 0; // Ensure it's a new entity
            subject.CreatedAt = DateTime.UtcNow;
            subject.UpdatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(subject);
            await _context.SaveChangesAsync();
            return subject;
        }
    }
}