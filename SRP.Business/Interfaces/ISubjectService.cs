using SRP.Model.DTOs.Requests;
using SRP.Model.Helper.Base;

namespace SRP.Business.Interfaces
{
    public interface ISubjectService
    {
        Task<ResultModel> AddOrUpdateSubjectAsync(SubjectRequest request);
        Task<ResultModel> GetSubjectByIdAsync(int subjectId);
        Task<ResultModel> GetSubjectByFilterAsync(SubjectFilterModel filter);
        Task<ResultModel> DeleteSubjectAsync(int subjectId);
    }
}

