using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface ITeacherService
    {
        Task<TeacherResponseDto> CreateTeacherAsync(TeacherRequest request, string createdBy);
        Task<TeacherResponseDto> GetTeacherByIdAsync(int teacherId);
        Task<IEnumerable<TeacherResponseDto>> GetAllTeachersAsync();
        Task<TeacherResponseDto> UpdateTeacherAsync(int teacherId, UpdateTeacherRequestDto request, string updatedBy);
        Task DeleteTeacherAsync(int teacherId);
        Task AssignSubjectToTeacherAsync(AssignSubjectToTeacherRequest request, string assignedBy);
        Task RemoveSubjectFromTeacherAsync(int teacherId, int subjectId);
        Task<IEnumerable<TeacherResponseDto>> GetTeachersByDepartmentAsync(int departmentId);
    }
}