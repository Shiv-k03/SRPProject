using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface IStudentService
    {
        Task<StudentResponse> CreateStudentAsync(StudentRequest request, string createdBy);
        Task<StudentResponse> GetStudentByIdAsync(int studentId);
        Task<IEnumerable<StudentResponse>> GetAllStudentsAsync();
        Task<StudentResponse> UpdateStudentAsync(int studentId, UpdateStudentRequestDto request, string updatedBy);
        Task DeleteStudentAsync(int studentId);
        Task AssignSubjectToStudentAsync(AssignSubjectToStudentRequest request, string assignedBy);
        Task RemoveSubjectFromStudentAsync(int studentId, int subjectId);
        Task<IEnumerable<StudentResponse>> GetStudentsByDepartmentAsync(int departmentId);
        Task<IEnumerable<StudentResponse>> GetStudentsBySubjectAsync(int subjectId);
    }
}