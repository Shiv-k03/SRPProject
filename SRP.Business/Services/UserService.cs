using SRP.Business.Interfaces;
using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;
using SRP.Repository.Entities;
using SRP.Repository.Enums;
using SRP.Repository.Interfaces;

namespace SRP.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;

        public UserService(
            IUserRepository userRepository,
            ITeacherRepository teacherRepository,
            IStudentRepository studentRepository)
        {
            _userRepository = userRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
        }

        // NEW: Create Admin user via API (no manual hashing)
        //public async Task<ResultModel> CreateAdminAsync(CreateAdminRequest request)
        //{
        //    try
        //    {
        //        // basic validation
        //        if (string.IsNullOrWhiteSpace(request.Username) ||
        //            string.IsNullOrWhiteSpace(request.Email) ||
        //            string.IsNullOrWhiteSpace(request.Password))
        //        {
        //            return ResultModel.Invalid("Username, Email and Password are required");
        //        }

        //        // duplicate username
        //        var byUsername = await _userRepository.GetByUsernameAsync(request.Username);
        //        if (byUsername != null && !byUsername.IsDeleted)
        //        {
        //            return ResultModel.Duplicate($"Username '{request.Username}' already exists");
        //        }

        //        // duplicate email
        //        var byEmail = await _userRepository.GetByEmailAsync(request.Email);
        //        if (byEmail != null && !byEmail.IsDeleted)
        //        {
        //            return ResultModel.Duplicate($"Email '{request.Email}' already exists");
        //        }

        //        var user = new User
        //        {
        //            Username = request.Username,
        //            Email = request.Email,
        //            FirstName = request.FirstName,
        //            LastName = request.LastName,
        //            Role = UserRole.Admin,
        //            PasswordHash = AuthService.HashPassword(request.Password),
        //            IsActive = true,
        //            IsDeleted = false,
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow,
        //            CreatedBy = "System",
        //            UpdatedBy = "System"
        //        };

        //        await _userRepository.AddAsync(user);

        //        return ResultModel.Success(null, "Admin user created successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return ResultModel.Exception("Error creating admin user", ex.Message);
        //    }
        //}

        public async Task<ResultModel> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return ResultModel.NotFound("User not found");
                }

                var response = new UserResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = user.Role,
                    RoleName = user.Role.ToString(),
                    IsActive = user.IsActive,
                    LastLogin = user.LastLogin,
                    CreatedAt = user.CreatedAt
                };

                return ResultModel.Success(response, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving user: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithDetailsAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return ResultModel.NotFound("User not found");
                }

                var profile = new UserProfileResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = user.Role,
                    RoleName = user.Role.ToString(),
                    IsActive = user.IsActive,
                    LastLogin = user.LastLogin
                };

                // Teacher/HOD profile
                if (user.Teacher != null && !user.Teacher.IsDeleted)
                {
                    var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(user.Teacher.TeacherId);
                    if (teacher != null)
                    {
                        profile.TeacherProfile = new TeacherResponse
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
                            CreatedAt = teacher.CreatedAt,
                            AssignedSubjects = teacher.TeacherSubjects
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
                                .ToList()
                        };
                    }
                }

                // Student profile
                if (user.Student != null && !user.Student.IsDeleted)
                {
                    var student = await _studentRepository.GetStudentWithDetailsAsync(user.Student.StudentId);
                    if (student != null)
                    {
                        profile.StudentProfile = new StudentResponse
                        {
                            StudentId = student.StudentId,
                            UserId = student.UserId,
                            Username = student.User.Username,
                            Email = student.User.Email,
                            FirstName = student.User.FirstName,
                            LastName = student.User.LastName,
                            FullName = $"{student.User.FirstName} {student.User.LastName}",
                            DepartmentId = student.DepartmentId,
                            DepartmentName = student.Department.DepartmentName,
                            RollNumber = student.RollNumber,
                            DateOfBirth = student.DateOfBirth,
                            Age = DateHelper.CalculateAge(student.DateOfBirth),
                            PhoneNumber = student.PhoneNumber,
                            Address = student.Address,
                            AdmissionDate = student.AdmissionDate,
                            CurrentSemester = student.CurrentSemester,
                            IsActive = student.User.IsActive,
                            CreatedAt = student.CreatedAt,
                            EnrolledSubjects = student.StudentSubjects
                                .Where(ss => !ss.IsDeleted && ss.Status == Constants.StudentSubjectStatus.Enrolled)
                                .Select(ss => new SubjectResponse
                                {
                                    SubjectId = ss.Subject.SubjectId,
                                    SubjectName = ss.Subject.SubjectName,
                                    SubjectCode = ss.Subject.SubjectCode,
                                    Credits = ss.Subject.Credits,
                                    Semester = ss.Subject.Semester,
                                    DepartmentId = ss.Subject.DepartmentId,
                                    DepartmentName = ss.Subject.Department.DepartmentName,
                                    Description = ss.Subject.Description,
                                    MaxMarks = ss.Subject.MaxMarks,
                                    PassingMarks = ss.Subject.PassingMarks,
                                    CreatedAt = ss.Subject.CreatedAt
                                })
                                .ToList()
                        };
                    }
                }

                return ResultModel.Success(profile, "User profile retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving user profile: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetUserByFilterAsync(UserFilterModel filter)
        {
            try
            {
                var query = _userRepository.GetQueryable()
                    .Where(u => !u.IsDeleted);

                if (!string.IsNullOrEmpty(filter.Username))
                    query = query.Where(u => u.Username.Contains(filter.Username));

                if (!string.IsNullOrEmpty(filter.Email))
                    query = query.Where(u => u.Email.Contains(filter.Email));

                if (filter.Role.HasValue)
                    query = query.Where(u => (int)u.Role == filter.Role.Value);

                if (filter.IsActive.HasValue)
                    query = query.Where(u => u.IsActive == filter.IsActive.Value);

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(u =>
                        u.Username.Contains(filter.SearchTerm) ||
                        u.Email.Contains(filter.SearchTerm) ||
                        u.FirstName.Contains(filter.SearchTerm) ||
                        u.LastName.Contains(filter.SearchTerm));
                }

                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "username" => filter.SortDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
                        "email" => filter.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                        "name" => filter.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                        _ => query.OrderBy(u => u.UserId)
                    };
                }

                var paginatedResult = PaginationHelper.CreatePaginationResult(
                    query,
                    filter.PageNumber,
                    filter.PageSize);

                var responses = paginatedResult.Items.Select(user => new UserResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = user.Role,
                    RoleName = user.Role.ToString(),
                    IsActive = user.IsActive,
                    LastLogin = user.LastLogin,
                    CreatedAt = user.CreatedAt
                }).ToList();

                var result = new
                {
                    Items = responses,
                    paginatedResult.TotalCount,
                    paginatedResult.PageNumber,
                    paginatedResult.PageSize,
                    paginatedResult.TotalPages
                };

                return ResultModel.Success(result, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<ResultModel> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return ResultModel.NotFound("User not found");
                }

                if (user.Email != request.Email)
                {
                    if (await _userRepository.IsEmailExistsAsync(request.Email))
                    {
                        return ResultModel.Duplicate($"Email '{request.Email}' already exists");
                    }
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.IsActive = request.IsActive;
                user.UpdatedBy = request.UpdatedBy;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                var response = new UserResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = user.Role,
                    RoleName = user.Role.ToString(),
                    IsActive = user.IsActive,
                    LastLogin = user.LastLogin,
                    CreatedAt = user.CreatedAt
                };

                return ResultModel.Updated(response, "User updated successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error updating user: {ex.Message}");
            }
        }

        public async Task<ResultModel> DeactivateUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return ResultModel.NotFound("User not found");
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                return ResultModel.Success(null, "User deactivated successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error deactivating user: {ex.Message}");
            }
        }

        public async Task<ResultModel> ActivateUserAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null || user.IsDeleted)
                {
                    return ResultModel.NotFound("User not found");
                }

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                return ResultModel.Success(null, "User activated successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error activating user: {ex.Message}");
            }
        }
    }
}

//using SRP.Business.Interfaces;
//using SRP.Model.DTOs.Requests;
//using SRP.Model.DTOs.Responses;
//using SRP.Model.Helper.Base;
//using SRP.Model.Helper.Helpers;
//using SRP.Repository.Interfaces;

//namespace SRP.Business.Services
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly ITeacherRepository _teacherRepository;
//        private readonly IStudentRepository _studentRepository;

//        public UserService(
//            IUserRepository userRepository,
//            ITeacherRepository teacherRepository,
//            IStudentRepository studentRepository)
//        {
//            _userRepository = userRepository;
//            _teacherRepository = teacherRepository;
//            _studentRepository = studentRepository;
//        }

//        public async Task<ResultModel> GetUserByIdAsync(int userId)
//        {
//            try
//            {
//                var user = await _userRepository.GetByIdAsync(userId);

//                if (user == null || user.IsDeleted)
//                {
//                    return ResultModel.NotFound("User not found");
//                }

//                var response = new UserResponse
//                {
//                    UserId = user.UserId,
//                    Username = user.Username,
//                    Email = user.Email,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    FullName = $"{user.FirstName} {user.LastName}",
//                    Role = user.Role,
//                    RoleName = user.Role.ToString(),
//                    IsActive = user.IsActive,
//                    LastLogin = user.LastLogin,
//                    CreatedAt = user.CreatedAt
//                };

//                return ResultModel.Success(response, "User retrieved successfully");
//            }
//            catch (Exception ex)
//            {
//                return ResultModel.Exception($"Error retrieving user: {ex.Message}");
//            }
//        }

//        public async Task<ResultModel> GetUserProfileAsync(int userId)
//        {
//            try
//            {
//                var user = await _userRepository.GetUserWithDetailsAsync(userId);

//                if (user == null || user.IsDeleted)
//                {
//                    return ResultModel.NotFound("User not found");
//                }

//                var profile = new UserProfileResponse
//                {
//                    UserId = user.UserId,
//                    Username = user.Username,
//                    Email = user.Email,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    FullName = $"{user.FirstName} {user.LastName}",
//                    Role = user.Role,
//                    RoleName = user.Role.ToString(),
//                    IsActive = user.IsActive,
//                    LastLogin = user.LastLogin
//                };

//                // Load teacher profile if user is teacher or HOD
//                if (user.Teacher != null && !user.Teacher.IsDeleted)
//                {
//                    var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(user.Teacher.TeacherId);
//                    if (teacher != null)
//                    {
//                        profile.TeacherProfile = new TeacherResponse
//                        {
//                            TeacherId = teacher.TeacherId,
//                            UserId = teacher.UserId,
//                            Username = teacher.User.Username,
//                            Email = teacher.User.Email,
//                            FirstName = teacher.User.FirstName,
//                            LastName = teacher.User.LastName,
//                            FullName = $"{teacher.User.FirstName} {teacher.User.LastName}",
//                            Role = teacher.User.Role,
//                            DepartmentId = teacher.DepartmentId,
//                            DepartmentName = teacher.Department.DepartmentName,
//                            EmployeeCode = teacher.EmployeeCode,
//                            Qualification = teacher.Qualification,
//                            PhoneNumber = teacher.PhoneNumber,
//                            JoiningDate = teacher.JoiningDate,
//                            IsActive = teacher.User.IsActive,
//                            CreatedAt = teacher.CreatedAt,
//                            AssignedSubjects = teacher.TeacherSubjects
//                                .Where(ts => !ts.IsDeleted)
//                                .Select(ts => new SubjectResponse
//                                {
//                                    SubjectId = ts.Subject.SubjectId,
//                                    SubjectName = ts.Subject.SubjectName,
//                                    SubjectCode = ts.Subject.SubjectCode,
//                                    Credits = ts.Subject.Credits,
//                                    Semester = ts.Subject.Semester,
//                                    DepartmentId = ts.Subject.DepartmentId,
//                                    DepartmentName = ts.Subject.Department.DepartmentName,
//                                    Description = ts.Subject.Description,
//                                    MaxMarks = ts.Subject.MaxMarks,
//                                    PassingMarks = ts.Subject.PassingMarks,
//                                    CreatedAt = ts.Subject.CreatedAt
//                                })
//                                .ToList()
//                        };
//                    }
//                }

//                // Load student profile if user is student
//                if (user.Student != null && !user.Student.IsDeleted)
//                {
//                    var student = await _studentRepository.GetStudentWithDetailsAsync(user.Student.StudentId);
//                    if (student != null)
//                    {
//                        profile.StudentProfile = new StudentResponse
//                        {
//                            StudentId = student.StudentId,
//                            UserId = student.UserId,
//                            Username = student.User.Username,
//                            Email = student.User.Email,
//                            FirstName = student.User.FirstName,
//                            LastName = student.User.LastName,
//                            FullName = $"{student.User.FirstName} {student.User.LastName}",
//                            DepartmentId = student.DepartmentId,
//                            DepartmentName = student.Department.DepartmentName,
//                            RollNumber = student.RollNumber,
//                            DateOfBirth = student.DateOfBirth,
//                            Age = DateHelper.CalculateAge(student.DateOfBirth),
//                            PhoneNumber = student.PhoneNumber,
//                            Address = student.Address,
//                            AdmissionDate = student.AdmissionDate,
//                            CurrentSemester = student.CurrentSemester,
//                            IsActive = student.User.IsActive,
//                            CreatedAt = student.CreatedAt,
//                            EnrolledSubjects = student.StudentSubjects
//                                .Where(ss => !ss.IsDeleted && ss.Status == Constants.StudentSubjectStatus.Enrolled)
//                                .Select(ss => new SubjectResponse
//                                {
//                                    SubjectId = ss.Subject.SubjectId,
//                                    SubjectName = ss.Subject.SubjectName,
//                                    SubjectCode = ss.Subject.SubjectCode,
//                                    Credits = ss.Subject.Credits,
//                                    Semester = ss.Subject.Semester,
//                                    DepartmentId = ss.Subject.DepartmentId,
//                                    DepartmentName = ss.Subject.Department.DepartmentName,
//                                    Description = ss.Subject.Description,
//                                    MaxMarks = ss.Subject.MaxMarks,
//                                    PassingMarks = ss.Subject.PassingMarks,
//                                    CreatedAt = ss.Subject.CreatedAt
//                                })
//                                .ToList()
//                        };
//                    }
//                }

//                return ResultModel.Success(profile, "User profile retrieved successfully");
//            }
//            catch (Exception ex)
//            {
//                return ResultModel.Exception($"Error retrieving user profile: {ex.Message}");
//            }
//        }

//        public async Task<ResultModel> GetUserByFilterAsync(UserFilterModel filter)
//        {
//            try
//            {
//                var query = _userRepository.GetQueryable()
//                    .Where(u => !u.IsDeleted);

//                // Apply filters
//                if (!string.IsNullOrEmpty(filter.Username))
//                {
//                    query = query.Where(u => u.Username.Contains(filter.Username));
//                }

//                if (!string.IsNullOrEmpty(filter.Email))
//                {
//                    query = query.Where(u => u.Email.Contains(filter.Email));
//                }

//                if (filter.Role.HasValue)
//                {
//                    query = query.Where(u => (int)u.Role == filter.Role.Value);
//                }

//                if (filter.IsActive.HasValue)
//                {
//                    query = query.Where(u => u.IsActive == filter.IsActive.Value);
//                }

//                if (!string.IsNullOrEmpty(filter.SearchTerm))
//                {
//                    query = query.Where(u =>
//                        u.Username.Contains(filter.SearchTerm) ||
//                        u.Email.Contains(filter.SearchTerm) ||
//                        u.FirstName.Contains(filter.SearchTerm) ||
//                        u.LastName.Contains(filter.SearchTerm));
//                }

//                // Apply sorting
//                if (!string.IsNullOrEmpty(filter.SortBy))
//                {
//                    query = filter.SortBy.ToLower() switch
//                    {
//                        "username" => filter.SortDescending ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
//                        "email" => filter.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
//                        "name" => filter.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
//                        _ => query.OrderBy(u => u.UserId)
//                    };
//                }

//                // Apply pagination
//                var paginatedResult = PaginationHelper.CreatePaginationResult(
//                    query,
//                    filter.PageNumber,
//                    filter.PageSize);

//                var responses = paginatedResult.Items.Select(user => new UserResponse
//                {
//                    UserId = user.UserId,
//                    Username = user.Username,
//                    Email = user.Email,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    FullName = $"{user.FirstName} {user.LastName}",
//                    Role = user.Role,
//                    RoleName = user.Role.ToString(),
//                    IsActive = user.IsActive,
//                    LastLogin = user.LastLogin,
//                    CreatedAt = user.CreatedAt
//                }).ToList();

//                var result = new
//                {
//                    Items = responses,
//                    paginatedResult.TotalCount,
//                    paginatedResult.PageNumber,
//                    paginatedResult.PageSize,
//                    paginatedResult.TotalPages
//                };

//                return ResultModel.Success(result, "Users retrieved successfully");
//            }
//            catch (Exception ex)
//            {
//                return ResultModel.Exception($"Error retrieving users: {ex.Message}");
//            }
//        }

//        public async Task<ResultModel> UpdateUserAsync(int userId, UpdateUserRequest request)
//        {
//            try
//            {
//                var user = await _userRepository.GetByIdAsync(userId);

//                if (user == null || user.IsDeleted)
//                {
//                    return ResultModel.NotFound("User not found");
//                }

//                // Check email uniqueness
//                if (user.Email != request.Email)
//                {
//                    if (await _userRepository.IsEmailExistsAsync(request.Email))
//                    {
//                        return ResultModel.Duplicate($"Email '{request.Email}' already exists");
//                    }
//                }

//                user.FirstName = request.FirstName;
//                user.LastName = request.LastName;
//                user.Email = request.Email;
//                user.IsActive = request.IsActive;
//                user.UpdatedBy = request.UpdatedBy;
//                user.UpdatedAt = DateTime.UtcNow;

//                await _userRepository.UpdateAsync(user);

//                var response = new UserResponse
//                {
//                    UserId = user.UserId,
//                    Username = user.Username,
//                    Email = user.Email,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    FullName = $"{user.FirstName} {user.LastName}",
//                    Role = user.Role,
//                    RoleName = user.Role.ToString(),
//                    IsActive = user.IsActive,
//                    LastLogin = user.LastLogin,
//                    CreatedAt = user.CreatedAt
//                };

//                return ResultModel.Updated(response, "User updated successfully");
//            }
//            catch (Exception ex)
//            {
//                return ResultModel.Exception($"Error updating user: {ex.Message}");
//            }
//        }

//        public async Task<ResultModel> DeactivateUserAsync(int userId)
//        {
//            try
//            {
//                var user = await _userRepository.GetByIdAsync(userId);

//                if (user == null || user.IsDeleted)
//                {
//                    return ResultModel.NotFound("User not found");
//                }

//                user.IsActive = false;
//                user.UpdatedAt = DateTime.UtcNow;

//                await _userRepository.UpdateAsync(user);

//                return ResultModel.Success(null, "User deactivated successfully");
//            }
//            catch (Exception ex)
//            {
//                return ResultModel.Exception($"Error deactivating user: {ex.Message}");
//            }
//        }

//        public async Task<ResultModel> ActivateUserAsync(int userId)
//        {
//            try
//            {
//                var user = await _userRepository.GetByIdAsync(userId);

//                if (user == null || user.IsDeleted)
//                {
//                    return ResultModel.NotFound("User not found");
//                }

//                user.IsActive = true;
//                user.UpdatedAt = DateTime.UtcNow;

//                await _userRepository.UpdateAsync(user);

//                return ResultModel.Success(null, "User activated successfully");
//            }
//            catch (Exception ex)
//            {
//                return ResultModel.Exception($"Error activating user: {ex.Message}");
//            }
//        }
//    }
//}