using SRP.Model.DTOs.Request;
using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Base;

namespace SRP.Business.Interfaces
{
    public interface IStudentService
    {
        Task<ResultModel> AddOrUpdateStudentAsync(StudentRequest request);
        Task<ResultModel> GetStudentByIdAsync(int studentId);
        Task<ResultModel> GetStudentByFilterAsync(StudentFilterModel filter);
        Task<ResultModel> DeleteStudentAsync(int studentId);
        Task<ResultModel> AssignSubjectToStudentAsync(AssignSubjectToStudentRequest request);
        Task<ResultModel> RemoveSubjectFromStudentAsync(int studentId, int subjectId);
        Task<ResultModel> GetStudentSummaryAsync(int studentId);
        Task<ResultModel> GetStudentDetailedReportAsync(int studentId);
    }
}