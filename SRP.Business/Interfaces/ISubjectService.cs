using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface ISubjectService
    {
        Task<SubjectResponseDto> CreateSubjectAsync(SubjectRequest request, string createdBy);
        Task<SubjectResponseDto> GetSubjectByIdAsync(int subjectId);
        Task<IEnumerable<SubjectResponseDto>> GetAllSubjectsAsync();
        Task<SubjectResponseDto> UpdateSubjectAsync(int subjectId, SubjectRequest request, string updatedBy);
        Task DeleteSubjectAsync(int subjectId);
        Task<IEnumerable<SubjectResponseDto>> GetSubjectsByDepartmentAsync(int departmentId);
        Task<IEnumerable<SubjectResponseDto>> GetSubjectsBySemesterAsync(int semester);
    }
}