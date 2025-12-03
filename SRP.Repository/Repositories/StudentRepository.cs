using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;
using StudentReportPortal.Infrastructure.Repositories;

namespace SRP.Repository.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task<Student?> GetStudentWithDetailsAsync(int studentId)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.StudentSubjects)
                    .ThenInclude(ss => ss.Subject)
                        .ThenInclude(sub => sub.Department)
                .Include(s => s.Marks)
                    .ThenInclude(m => m.Subject)
                .FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);
        }

        public async Task<Student?> GetStudentByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.StudentSubjects)
                    .ThenInclude(ss => ss.Subject)
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
        }

        public async Task<Student?> GetStudentByRollNumberAsync(string rollNumber)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Department)
                .FirstOrDefaultAsync(s => s.RollNumber == rollNumber && !s.IsDeleted);
        }

        public async Task<IEnumerable<Student>> GetStudentsByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Department)
                .Where(s => s.DepartmentId == departmentId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsBySubjectAsync(int subjectId)
        {
            return await _dbSet
                .Include(s => s.User)
                .Include(s => s.Department)
                .Include(s => s.StudentSubjects)
                .Where(s => s.StudentSubjects.Any(ss => ss.SubjectId == subjectId) && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsRollNumberExistsAsync(string rollNumber)
        {
            return await _dbSet.AnyAsync(s => s.RollNumber == rollNumber && !s.IsDeleted);
        }
    }
}