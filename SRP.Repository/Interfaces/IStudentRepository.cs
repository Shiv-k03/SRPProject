using SRP.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRP.Repository.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetStudentWithDetailsAsync(int studentId);
        Task<Student?> GetStudentByUserIdAsync(int userId);
        Task<Student?> GetStudentByRollNumberAsync(string rollNumber);
        Task<IEnumerable<Student>> GetStudentsByDepartmentAsync(int departmentId);
        Task<IEnumerable<Student>> GetStudentsBySubjectAsync(int subjectId);
        Task<bool> IsRollNumberExistsAsync(string rollNumber);
        Task<Student> AddOrUpdateStudentAsync(Student student, User user);
    }
}
