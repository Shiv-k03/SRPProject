using AutoMapper;
using SRP.Business.Interfaces;
using SRP.Model.DTOs.Responses;
using SRP.Repository.Exceptions;
using SRP.API.Helper.Helpers;
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

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException(Constants.ErrorMessages.UserNotFound);
            }

            return new UserResponseDto
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
        }

        public async Task<UserProfileResponseDto> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException(Constants.ErrorMessages.UserNotFound);
            }

            var profile = new UserProfileResponseDto
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

            // Load teacher or student profile based on role
            if (user.Teacher != null && !user.Teacher.IsDeleted)
            {
                var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(user.Teacher.TeacherId);
                if (teacher != null)
                {
                    var assignedSubjects = teacher.TeacherSubjects
                        .Where(ts => !ts.IsDeleted)
                        .Select(ts => new SubjectResponseDto
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

                    profile.TeacherProfile = new TeacherResponseDto
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
                        AssignedSubjects = assignedSubjects,
                        CreatedAt = teacher.CreatedAt
                    };
                }
            }

            if (user.Student != null && !user.Student.IsDeleted)
            {
                var student = await _studentRepository.GetStudentWithDetailsAsync(user.Student.StudentId);
                if (student != null)
                {
                    var enrolledSubjects = student.StudentSubjects
                        .Where(ss => !ss.IsDeleted && ss.Status == Constants.StudentSubjectStatus.Enrolled)
                        .Select(ss => new SubjectResponseDto
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
                        .ToList();

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
                        EnrolledSubjects = enrolledSubjects,
                        CreatedAt = student.CreatedAt
                    };
                }
            }

            return profile;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var responses = new List<UserResponseDto>();

            foreach (var user in users.Where(u => !u.IsDeleted))
            {
                responses.Add(new UserResponseDto
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
                });
            }

            return responses;
        }

        public async Task<UserResponseDto> UpdateUserAsync(int userId, UpdateUserRequestDto request, string updatedBy)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException(Constants.ErrorMessages.UserNotFound);
            }

            if (user.Email != request.Email)
            {
                if (await _userRepository.IsEmailExistsAsync(request.Email))
                {
                    throw new BadRequestException($"Email '{request.Email}' already exists");
                }
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.IsActive = request.IsActive;
            user.UpdatedBy = updatedBy;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return await GetUserByIdAsync(userId);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException(Constants.ErrorMessages.UserNotFound);
            }

            // Verify current password
            var currentPasswordHash = AuthService.HashPassword(request.CurrentPassword);
            if (currentPasswordHash != user.PasswordHash)
            {
                throw new BadRequestException("Current password is incorrect");
            }

            // Update password
            user.PasswordHash = AuthService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException(Constants.ErrorMessages.UserNotFound);
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }

        public async Task ActivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || user.IsDeleted)
            {
                throw new NotFoundException(Constants.ErrorMessages.UserNotFound);
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }
    }
}