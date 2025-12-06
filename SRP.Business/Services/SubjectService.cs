using SRP.Business.Interfaces;
using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;

namespace SRP.Business.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public SubjectService(
        ISubjectRepository subjectRepository,
        IDepartmentRepository departmentRepository)
        {
            _subjectRepository = subjectRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<ResultModel> AddOrUpdateSubjectAsync(SubjectRequest request)
        {
            try
            {
                // Validate department
                var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
                if (department == null || department.IsDeleted)
                {
                    return ResultModel.NotFound("Department not found");
                }

                // Check for duplicate subject code (excluding current subject if updating)
                var existingSubject = await _subjectRepository.GetSubjectByCodeAsync(request.SubjectCode);
                if (existingSubject != null && existingSubject.SubjectId != request.SubjectId)
                {
                    return ResultModel.Duplicate($"Subject code '{request.SubjectCode}' already exists");
                }

                var subject = new Subject
                {
                    SubjectId = request.SubjectId ?? 0,
                    SubjectName = request.SubjectName,
                    SubjectCode = request.SubjectCode,
                    Credits = request.Credits,
                    Semester = request.Semester,
                    DepartmentId = request.DepartmentId,
                    Description = request.Description,
                    MaxMarks = request.MaxMarks,
                    PassingMarks = request.PassingMarks,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                var savedSubject = await _subjectRepository.AddOrUpdateSubjectAsync(subject);

                var response = await GetSubjectResponseAsync(savedSubject.SubjectId);

                if (request.SubjectId.HasValue && request.SubjectId.Value > 0)
                {
                    return ResultModel.Updated(response, "Subject updated successfully");
                }
                else
                {
                    return ResultModel.Created(response, "Subject created successfully");
                }
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error adding/updating subject: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetSubjectByIdAsync(int subjectId)
        {
            try
            {
                var subject = await _subjectRepository.GetSubjectWithDetailsAsync(subjectId);

                if (subject == null)
                {
                    return ResultModel.NotFound("Subject not found");
                }

                var response = await GetSubjectResponseAsync(subjectId);

                return ResultModel.Success(response, "Subject retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving subject: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetSubjectByFilterAsync(SubjectFilterModel filter)
        {
            try
            {
                var query = _subjectRepository.GetQueryable()
                .Where(s => !s.IsDeleted);

                // Apply filters
                if (!string.IsNullOrEmpty(filter.SubjectCode))
                {
                    query = query.Where(s => s.SubjectCode.Contains(filter.SubjectCode));
                }

                if (filter.DepartmentId.HasValue)
                {
                    query = query.Where(s => s.DepartmentId == filter.DepartmentId.Value);
                }

                if (filter.Semester.HasValue)
                {
                    query = query.Where(s => s.Semester == filter.Semester.Value);
                }

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(s =>
                    s.SubjectName.Contains(filter.SearchTerm) ||
                    s.SubjectCode.Contains(filter.SearchTerm));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "name" => filter.SortDescending ? query.OrderByDescending(s => s.SubjectName) : query.OrderBy(s => s.SubjectName),
                        "code" => filter.SortDescending ? query.OrderByDescending(s => s.SubjectCode) : query.OrderBy(s => s.SubjectCode),
                        _ => query.OrderBy(s => s.SubjectId)
                    };
                }

                // Apply pagination
                var paginatedResult = PaginationHelper.CreatePaginationResult(
                query,
                filter.PageNumber,
                filter.PageSize);

                var responses = new List<SubjectResponse>();
                foreach (var subject in paginatedResult.Items)
                {
                    responses.Add(await GetSubjectResponseAsync(subject.SubjectId));
                }

                var result = new
                {
                    Items = responses,
                    paginatedResult.TotalCount,
                    paginatedResult.PageNumber,
                    paginatedResult.PageSize,
                    paginatedResult.TotalPages
                };

                return ResultModel.Success(result, "Subjects retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving subjects: {ex.Message}");
            }
        }

        public async Task<ResultModel> DeleteSubjectAsync(int subjectId)
        {
            try
            {
                var subject = await _subjectRepository.GetByIdAsync(subjectId);

                if (subject == null || subject.IsDeleted)
                {
                    return ResultModel.NotFound("Subject not found");
                }

                subject.IsDeleted = true;
                subject.UpdatedAt = DateTime.UtcNow;

                await _subjectRepository.UpdateAsync(subject);

                return ResultModel.Success(null, "Subject deleted successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error deleting subject: {ex.Message}");
            }
        }

        private async Task<SubjectResponse> GetSubjectResponseAsync(int subjectId)
        {
            var subject = await _subjectRepository.GetSubjectWithDetailsAsync(subjectId);

            return new SubjectResponse
            {
                SubjectId = subject.SubjectId,
                SubjectName = subject.SubjectName,
                SubjectCode = subject.SubjectCode,
                Credits = subject.Credits,
                Semester = subject.Semester,
                DepartmentId = subject.DepartmentId,
                DepartmentName = subject.Department.DepartmentName,
                Description = subject.Description,
                MaxMarks = subject.MaxMarks,
                PassingMarks = subject.PassingMarks,
                CreatedAt = subject.CreatedAt
            };
        }
    }
}