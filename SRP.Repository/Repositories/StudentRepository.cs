using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SRP.Repository.Context;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

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

        public async Task<Student> AddOrUpdateStudentAsync(Student student, User user)
        {
            // Check if updating by StudentId
            if (student.StudentId > 0)
            {
                var existingStudent = await _dbSet
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.StudentId == student.StudentId && !s.IsDeleted);

                if (existingStudent != null)
                {
                    // Update existing student
                    existingStudent.DepartmentId = student.DepartmentId;
                    existingStudent.RollNumber = student.RollNumber;
                    existingStudent.DateOfBirth = student.DateOfBirth;
                    existingStudent.PhoneNumber = student.PhoneNumber;
                    existingStudent.Address = student.Address;
                    existingStudent.AdmissionDate = student.AdmissionDate;
                    existingStudent.CurrentSemester = student.CurrentSemester;
                    existingStudent.UpdatedBy = student.UpdatedBy;
                    existingStudent.UpdatedAt = DateTime.UtcNow;

                    // Update user information
                    existingStudent.User.FirstName = user.FirstName;
                    existingStudent.User.LastName = user.LastName;
                    existingStudent.User.Email = user.Email;
                    existingStudent.User.UpdatedAt = DateTime.UtcNow;
                    existingStudent.User.UpdatedBy = user.UpdatedBy;

                    // Update password only if provided
                    if (!string.IsNullOrEmpty(user.PasswordHash))
                    {
                        existingStudent.User.PasswordHash = user.PasswordHash;
                    }

                    _dbSet.Update(existingStudent);
                    await _context.SaveChangesAsync();
                    return existingStudent;
                }
            }

            // Add new student with user
            user.UserId = 0; // Ensure it's a new entity
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            student.StudentId = 0; // Ensure it's a new entity
            student.UserId = user.UserId;
            student.CreatedAt = DateTime.UtcNow;
            student.UpdatedAt = DateTime.UtcNow;

            await _dbSet.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }
    }
}