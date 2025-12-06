using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Base;

namespace SRP.Business.Interfaces
{
    public interface ITeacherService
    {
        Task<ResultModel> AddOrUpdateTeacherAsync(TeacherRequest request);
        Task<ResultModel> GetTeacherByIdAsync(int teacherId);
        Task<ResultModel> GetTeacherByFilterAsync(TeacherFilterModel filter);
        Task<ResultModel> DeleteTeacherAsync(int teacherId);
        Task<ResultModel> AssignSubjectToTeacherAsync(AssignSubjectToTeacherRequest request);
        Task<ResultModel> RemoveSubjectFromTeacherAsync(int teacherId, int subjectId);
        Task<ResultModel> GetTeacherSummaryAsync(int teacherId);
    }
}