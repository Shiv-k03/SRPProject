using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace StudentReportPortal.Infrastructure.Repositories
{
    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(ApplicationDbContext context, IMapper mapper) 
            : base(context , mapper)
        {
        }

        public async Task<Teacher?> GetTeacherWithDetailsAsync(int teacherId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                        .ThenInclude(s => s.Department)
                .FirstOrDefaultAsync(t => t.TeacherId == teacherId && !t.IsDeleted);
        }

        public async Task<Teacher?> GetTeacherByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.TeacherSubjects)
                    .ThenInclude(ts => ts.Subject)
                .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
        }

        public async Task<Teacher?> GetTeacherByEmployeeCodeAsync(string employeeCode)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.EmployeeCode == employeeCode && !t.IsDeleted);
        }

        public async Task<IEnumerable<Teacher>> GetTeachersByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.Department)
                .Where(t => t.DepartmentId == departmentId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsEmployeeCodeExistsAsync(string employeeCode)
        {
            return await _dbSet.AnyAsync(t => t.EmployeeCode == employeeCode && !t.IsDeleted);
        }
    }
}