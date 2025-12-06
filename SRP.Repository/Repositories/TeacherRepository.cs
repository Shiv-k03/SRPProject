using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace SRP.Repository.Repositories
{
    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
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

        public async Task<Teacher> AddOrUpdateTeacherAsync(Teacher teacher, User user)
        {
            // Check if updating by TeacherId
            if (teacher.TeacherId > 0)
            {
                var existingTeacher = await _dbSet
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.TeacherId == teacher.TeacherId && !t.IsDeleted);

                if (existingTeacher != null)
                {
                    // Update existing teacher
                    existingTeacher.DepartmentId = teacher.DepartmentId;
                    existingTeacher.EmployeeCode = teacher.EmployeeCode;
                    existingTeacher.Qualification = teacher.Qualification;
                    existingTeacher.PhoneNumber = teacher.PhoneNumber;
                    existingTeacher.JoiningDate = teacher.JoiningDate;
                    existingTeacher.UpdatedBy = teacher.UpdatedBy;
                    existingTeacher.UpdatedAt = DateTime.UtcNow;

                    // Update user information
                    existingTeacher.User.FirstName = user.FirstName;
                    existingTeacher.User.LastName = user.LastName;
                    existingTeacher.User.Email = user.Email;
                    existingTeacher.User.Role = user.Role;
                    existingTeacher.User.UpdatedAt = DateTime.UtcNow;
                    existingTeacher.User.UpdatedBy = user.UpdatedBy;

                    // Update password only if provided
                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        existingTeacher.User.PasswordHash = user.PasswordHash;
                    }

                    _dbSet.Update(existingTeacher);
                    await _context.SaveChangesAsync();
                    return existingTeacher;
                }
            }

            // Add new teacher with user
            user.UserId = 0; // Ensure it's a new entity
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            teacher.TeacherId = 0; // Ensure it's a new entity
            teacher.UserId = user.UserId;
            teacher.CreatedAt = DateTime.UtcNow;
            teacher.UpdatedAt = DateTime.UtcNow;

            await _dbSet.AddAsync(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }
    }
}