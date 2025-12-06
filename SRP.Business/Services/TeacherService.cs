using SRP.Business.Interfaces;
using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Repository.Entities;
using SRP.Repository.Enums;
using SRP.Repository.Interfaces;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;

namespace SRP.Business.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherSubjectRepository _teacherSubjectRepository;


        public TeacherService(
            IUserRepository userRepository, 
            IDepartmentRepository departmentRepository, 
            ISubjectRepository subjectRepository, 
            ITeacherSubjectRepository teacherSubjectRepository,
            ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _subjectRepository = subjectRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
        }

        public async Task<ResultModel> AddOrUpdateTeacherAsync(TeacherRequest request)
        {
            try
            {
                // Validate role
                if (request.Role != UserRole.HOD && request.Role != UserRole.Teacher)
                {
                    return ResultModel.Invalid("Role must be either HOD or Teacher");
                }

                // Validate department
                var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
                if (department == null || department.IsDeleted)
                {
                    return ResultModel.NotFound("Department not found");
                }

                // Check for duplicate username (excluding current user if updating)
                var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
                if (existingUser != null && existingUser.Teacher?.TeacherId != request.TeacherId)
                {
                    return ResultModel.Duplicate($"Username '{request.Username}' already exists");
                }

                // Check for duplicate email (excluding current user if updating)
                var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingEmail != null && existingEmail.Teacher?.TeacherId != request.TeacherId)
                {
                    return ResultModel.Duplicate($"Email '{request.Email}' already exists");
                }

                // Check for duplicate employee code (excluding current teacher if updating)
                var existingEmpCode = await _teacherRepository.GetTeacherByEmployeeCodeAsync(request.EmployeeCode);
                if (existingEmpCode != null && existingEmpCode.TeacherId != request.TeacherId)
                {
                    return ResultModel.Duplicate($"Employee code '{request.EmployeeCode}' already exists");
                }

                // Create User entity
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = request.Role,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                // Hash password only if provided (for new users or password change)
                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.PasswordHash = AuthService.HashPassword(request.Password);
                }

                // Create Teacher entity
                var teacher = new Teacher
                {
                    TeacherId = request.TeacherId ?? 0,
                    DepartmentId = request.DepartmentId,
                    EmployeeCode = request.EmployeeCode,
                    Qualification = request.Qualification,
                    PhoneNumber = request.PhoneNumber,
                    JoiningDate = request.JoiningDate,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                var savedTeacher = await _teacherRepository.AddOrUpdateTeacherAsync(teacher, user);

                var response = await GetTeacherResponseAsync(savedTeacher.TeacherId);

                if (request.TeacherId.HasValue && request.TeacherId.Value > 0)
                {
                    return ResultModel.Updated(response, "Teacher updated successfully");
                }
                else
                {
                    return ResultModel.Created(response, "Teacher created successfully");
                }
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error adding/updating teacher: {ex.Message}");
            }
        }


        public async Task<ResultModel> GetTeacherByIdAsync(int teacherId)
        {
            try
            {
                var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(teacherId);

                if (teacher == null)
                {
                    return ResultModel.NotFound("Teacher Not Found");
                }
                var response = await GetTeacherResponseAsync(teacherId);

                return ResultModel.Success(response, "Teacher Retrievedsuccessfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving teacher: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetTeacherByFilterAsync(TeacherFilterModel filter)
        {
            try
            {
                var query = _teacherRepository.GetQueryable()
                    .Where(t => !t.IsDeleted);


                if (!string.IsNullOrEmpty(filter.EmployeeCode))
                {
                    query = query.Where(t => t.EmployeeCode.Contains(filter.EmployeeCode));
                }

                if (filter.DepartmentId.HasValue)
                {
                    query = query.Where(t=> t.DepartmentId == filter.DepartmentId.Value);
                }

                if (filter.Role.HasValue)
                {
                    query = query.Where(t => (int)t.User.Role == filter.Role.Value);
                }

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(t =>
                    t.User.FirstName.Contains(filter.SearchTerm) ||
                    t.User.LastName.Contains(filter.SearchTerm) ||
                    t.EmployeeCode.Contains(filter.SearchTerm));

                }

                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "name" => filter.SortDescending ? query.OrderByDescending(t => t.User.FirstName) : query.OrderBy(t => t.User.FirstName),
                        "code" => filter.SortDescending ? query.OrderByDescending(t => t.EmployeeCode) : query.OrderBy(t => t.EmployeeCode),
                        _ => query.OrderBy(t => t.TeacherId)
                    };
                }


                //Pagination
                var paginationResult = PaginationHelper.CreatePaginationResult(
                    query,
                    filter.PageNumber,
                    filter.PageSize);

                var response = new List<TeacherResponse>();
                foreach (var teacher in paginationResult.Items)
                {
                    response.Add(await GetTeacherResponseAsync(teacher.TeacherId));
                }

                var result = new
                {
                    Items = response,
                    paginationResult.TotalCount,
                    paginationResult.TotalPages,
                    paginationResult.PageSize,
                    paginationResult.PageNumber
                };

                return ResultModel.Success(result, "Teacher retrieved successfully");

            }
            catch (Exception ex) 
            {
                return ResultModel.Exception($"Error retrieving teachers : {ex.Message}");            
            }
        }

        public async Task<ResultModel> DeleteTeacherAsync(int teacherId)
        {
            try
            {
                var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(teacherId);

                if (teacher == null)
                {
                    return ResultModel.NotFound("Teacher not Found");
                }

                teacher.User.IsDeleted = true;
                teacher.User.IsActive = false;
                teacher.User.UpdatedAt = DateTime.Now;
                await _userRepository.UpdateAsync(teacher.User);

                teacher.IsDeleted = true;
                teacher.UpdatedAt = DateTime.Now;
                await _teacherRepository.UpdateAsync(teacher);

                return ResultModel.Success(null, "Teacher deleted Sucessfully");
            }
            catch (Exception ex) 
            {
                return ResultModel.Exception($"Error deleting teacher : {ex.Message}");
            } 
        }

        public async Task<ResultModel> AssignSubjectToTeacherAsync(AssignSubjectToTeacherRequest request)
        {
            try 
            {
                var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);

                if (teacher == null || teacher.IsDeleted)
                {
                    return ResultModel.NotFound("Teacher not Found");
                }

                var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);

                if (subject == null || subject.IsDeleted)
                {
                    return ResultModel.NotFound("Subject not found");
                }

                if (await _teacherSubjectRepository.IsTeacherAssignedToSubjectAsync(request.TeacherId, request.SubjectId, request.Semester))
                {
                    return ResultModel.Duplicate("Teacher is already assigned to this subject for the specified semester");
                }

                var teacherSubject = new TeacherSubject
                {
                    TeacherId = request.TeacherId,
                    SubjectId = request.SubjectId,
                    Semester = request.Semester,
                    AcademicYear = request.AcademicYear,
                    AssignedDate = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                await _teacherSubjectRepository.AddAsync(teacherSubject);

                return ResultModel.Created(null, "Subject assigned to teacher successfully");
            }
            catch (Exception ex) 
            {
                return ResultModel.Exception($"Error assigning subject to teacher: {ex.Message}");
            } 
        }

        public async Task<ResultModel> RemoveSubjectFromTeacherAsync(int teacherId, int subjectId)
        {
            try
            {
                var teacherSubjects = await _teacherSubjectRepository.GetSubjectsByTeacherAsync(teacherId);
                var teacherSubject = teacherSubjects.FirstOrDefault(ts => ts.SubjectId == subjectId && !ts.IsDeleted);

                if (teacherSubject == null)
                {
                    return ResultModel.NotFound("Teacher subject assignment not found");
                }

                teacherSubject.IsDeleted = true;
                teacherSubject.UpdatedAt = DateTime.UtcNow;

                await _teacherSubjectRepository.UpdateAsync(teacherSubject);

                return ResultModel.Success("Subject removed from teacher sucessfully");
            }
            catch (Exception ex) 
            {
                return ResultModel.Exception($"Error removing subject from teacher: {ex.Message}");
            }         
        }

        public async Task<ResultModel> GetTeacherSummaryAsync(int teacherId)
        {
            try
            {
                var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(teacherId);

                if (teacher == null)
                {
                    return ResultModel.NotFound("Teacher not found");
                }

                var response = new TeacherSummaryResponse
                {
                    TeacherId = teacher.TeacherId,
                    EmployeeCode = teacher.EmployeeCode,
                    FullName = $"{teacher.User.FirstName} {teacher.User.LastName}",
                    Email = teacher.User.Email,
                    Role = teacher.User.Role,
                    RoleName = teacher.User.Role.ToString(),
                    DepartmentName = teacher.Department.DepartmentName,
                    Qualification = teacher.Qualification,
                    TotalSubjectsAssigned = teacher.TeacherSubjects.Count(ts => !ts.IsDeleted),
                    SubjectNames = teacher.TeacherSubjects
                        .Where(ts => !ts.IsDeleted)
                        .Select(ts => ts.Subject.SubjectName)
                        .ToList(),
                    JoiningDate = teacher.JoiningDate,
                    IsActive = teacher.User.IsActive
                };

                return ResultModel.Success(response, "Teacher summary retrieved successfully");
            }
            catch (Exception ex) 
            {
                return ResultModel.Exception($"Error retrieving teacher summary: {ex.Message}");
            } 
        }

        private async Task<TeacherResponse> GetTeacherResponseAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(teacherId);

            var response = new TeacherResponse
            {
                TeacherId = teacher.TeacherId,
                UserId = teacher.UserId,
                Username = teacher.User.Username,
                Email = teacher.User.Email,
                FirstName = teacher.User.FirstName,
                LastName = teacher.User.LastName,
                FullName = $"{teacher.User.FirstName} {teacher.User.LastName}",
                Role = teacher.User.Role,
                DepartmentId = teacher.DepartmentId,
                DepartmentName = teacher.Department.DepartmentName,
                EmployeeCode = teacher.EmployeeCode,
                Qualification = teacher.Qualification,
                PhoneNumber = teacher.PhoneNumber,
                JoiningDate = teacher.JoiningDate,
                IsActive = teacher.User.IsActive,
                CreatedAt = teacher.CreatedAt
            };

            response.AssignedSubjects = teacher.TeacherSubjects
                .Where(ts => !ts.IsDeleted)
                .Select(ts => new SubjectResponse
                {
                    SubjectId = ts.Subject.SubjectId,
                    SubjectName = ts.Subject.SubjectName,
                    SubjectCode = ts.Subject.SubjectCode,
                    Credits = ts.Subject.Credits,
                    Semester = ts.Subject.Semester,
                    DepartmentId = ts.Subject.DepartmentId,
                    DepartmentName = ts.Subject.Department.DepartmentName,
                    Description = ts.Subject.Description,
                    MaxMarks = ts.Subject.MaxMarks,
                    PassingMarks = ts.Subject.PassingMarks,
                    CreatedAt = ts.Subject.CreatedAt
                })
                .ToList();

            return response;
        }

    }

}
