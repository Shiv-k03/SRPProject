using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;

namespace SRP.Business.Interfaces
{
    public interface IMarkService
    {
 

        Task<MarkResponse> UpdateMarksAsync(MarkRequest request, int teacherId, string updatedBy);
        Task<IEnumerable<MarkResponse>> GetMarksByStudentAsync(int studentId);
        Task<IEnumerable<MarkResponse>> GetMarksBySubjectAsync(int subjectId);
        Task<MarkResponse> GetMarkByIdAsync(int markId);
    }
}