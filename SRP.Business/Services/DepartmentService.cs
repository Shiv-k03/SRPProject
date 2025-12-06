using SRP.Business.Interfaces;
using SRP.Model.DTOs.Request;
using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;

namespace SRP.Business.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ITeacherRepository _teacherRepository;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            ITeacherRepository teacherRepository)
        {
            _departmentRepository = departmentRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<ResultModel> AddOrUpdateDepartmentAsync(DepartmentRequest request)
        {
            try
            {
                // Validate HOD if provided
                if (request.HeadOfDepartmentId.HasValue)
                {
                    var hod = await _teacherRepository.GetByIdAsync(request.HeadOfDepartmentId.Value);
                    if (hod == null || hod.IsDeleted)
                    {
                        return ResultModel.NotFound("Head of Department not found");
                    }
                }

                // Check for duplicate department code (excluding current department if updating)
                var existingDept = await _departmentRepository.GetByCodeAsync(request.DepartmentCode);
                if (existingDept != null && existingDept.DepartmentId != request.DepartmentId)
                {
                    return ResultModel.Duplicate($"Department code '{request.DepartmentCode}' already exists");
                }

                var department = new Department
                {
                    DepartmentId = request.DepartmentId ?? 0,
                    DepartmentName = request.DepartmentName,
                    DepartmentCode = request.DepartmentCode,
                    DepartmentType = request.DepartmentType,
                    Description = request.Description,
                    HeadOfDepartmentId = request.HeadOfDepartmentId,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                var savedDepartment = await _departmentRepository.AddOrUpdateDepartmentAsync(department);

                var response = await GetDepartmentResponseAsync(savedDepartment.DepartmentId);

                if (request.DepartmentId.HasValue && request.DepartmentId.Value > 0)
                {
                    return ResultModel.Updated(response, "Department updated successfully");
                }
                else
                {
                    return ResultModel.Created(response, "Department created successfully");
                }
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error adding/updating department: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetDepartmentByIdAsync(int departmentId)
        {
            try
            {
                var department = await _departmentRepository.GetDepartmentWithDetailsAsync(departmentId);

                if (department == null)
                {
                    return ResultModel.NotFound("Department not found");
                }

                var response = await GetDepartmentResponseAsync(departmentId);

                return ResultModel.Success(response, "Department retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving department: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetDepartmentByFilterAsync(DepartmentFilterModel filter)
        {
            try
            {
                var query = _departmentRepository.GetQueryable()
                    .Where(d => !d.IsDeleted);

                // Apply filters
                if (!string.IsNullOrEmpty(filter.DepartmentName))
                {
                    query = query.Where(d => d.DepartmentName.Contains(filter.DepartmentName));
                }

                if (!string.IsNullOrEmpty(filter.DepartmentCode))
                {
                    query = query.Where(d => d.DepartmentCode.Contains(filter.DepartmentCode));
                }

                if (filter.DepartmentType.HasValue)
                {
                    query = query.Where(d => (int)d.DepartmentType == filter.DepartmentType.Value);
                }

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(d =>
                        d.DepartmentName.Contains(filter.SearchTerm) ||
                        d.DepartmentCode.Contains(filter.SearchTerm));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "name" => filter.SortDescending ? query.OrderByDescending(d => d.DepartmentName) : query.OrderBy(d => d.DepartmentName),
                        "code" => filter.SortDescending ? query.OrderByDescending(d => d.DepartmentCode) : query.OrderBy(d => d.DepartmentCode),
                        _ => query.OrderBy(d => d.DepartmentId)
                    };
                }

                // Apply pagination
                var paginatedResult = PaginationHelper.CreatePaginationResult(
                    query,
                    filter.PageNumber,
                    filter.PageSize);

                var responses = new List<DepartmentResponse>();
                foreach (var dept in paginatedResult.Items)
                {
                    responses.Add(await GetDepartmentResponseAsync(dept.DepartmentId));
                }

                var result = new
                {
                    Items = responses,
                    paginatedResult.TotalCount,
                    paginatedResult.PageNumber,
                    paginatedResult.PageSize,
                    paginatedResult.TotalPages
                };

                return ResultModel.Success(result, "Departments retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving departments: {ex.Message}");
            }
        }

        public async Task<ResultModel> DeleteDepartmentAsync(int departmentId)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(departmentId);

                if (department == null || department.IsDeleted)
                {
                    return ResultModel.NotFound("Department not found");
                }

                department.IsDeleted = true;
                department.UpdatedAt = DateTime.UtcNow;

                await _departmentRepository.UpdateAsync(department);

                return ResultModel.Success(null, "Department deleted successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error deleting department: {ex.Message}");
            }
        }

        private async Task<DepartmentResponse> GetDepartmentResponseAsync(int departmentId)
        {
            var department = await _departmentRepository.GetDepartmentWithDetailsAsync(departmentId);

            var response = new DepartmentResponse
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                DepartmentCode = department.DepartmentCode,
                DepartmentType = department.DepartmentType,
                Description = department.Description,
                HeadOfDepartmentId = department.HeadOfDepartmentId,
                TotalTeachers = department.Teachers.Count(t => !t.IsDeleted),
                TotalStudents = department.Students.Count(s => !s.IsDeleted),
                TotalSubjects = department.Subjects.Count(s => !s.IsDeleted),
                CreatedAt = department.CreatedAt
            };

            if (department.HeadOfDepartmentId.HasValue)
            {
                var hod = department.Teachers.FirstOrDefault(t => t.TeacherId == department.HeadOfDepartmentId.Value);
                if (hod != null)
                {
                    response.HeadOfDepartmentName = $"{hod.User.FirstName} {hod.User.LastName}";
                }
            }

            return response;
        }
    }
}