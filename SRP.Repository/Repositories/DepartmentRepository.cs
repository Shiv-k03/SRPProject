using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace StudentReportPortal.Infrastructure.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context, IMapper mapper)
                    : base(context, mapper)
        {

        }

        public async Task<Department?> GetDepartmentWithDetailsAsync(int departmentId)
        {
            return await _dbSet
                .Include(d => d.Teachers)
                    .ThenInclude(t => t.User)
                .Include(d => d.Students)
                    .ThenInclude(s => s.User)
                .Include(d => d.Subjects)
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId && !d.IsDeleted);
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsWithDetailsAsync()
        {
            return await _dbSet
                .Include(d => d.Teachers)
                .Include(d => d.Students)
                .Include(d => d.Subjects)
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsDepartmentCodeExistsAsync(string departmentCode)
        {
            return await _dbSet.AnyAsync(d => d.DepartmentCode == departmentCode && !d.IsDeleted);
        }

        public async Task<Department?> GetByCodeAsync(string departmentCode)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.DepartmentCode == departmentCode && !d.IsDeleted);
        }
    }
}