using SRP.Business.Interfaces;
using SRP.Model.DTOs.Request;
using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Repository.Entities;
using SRP.Repository.Enums;
using SRP.Repository.Interfaces;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;

namespace SRP.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IStudentSubjectRepository _studentSubjectRepository;
        private readonly IMarkRepository _markRepository;

        public StudentService(
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            ISubjectRepository subjectRepository,
            IStudentSubjectRepository studentSubjectRepository,
            IMarkRepository markRepository)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _subjectRepository = subjectRepository;
            _studentSubjectRepository = studentSubjectRepository;
            _markRepository = markRepository;
        }

        public async Task<ResultModel> AddOrUpdateStudentAsync(StudentRequest request)
        {
            try
            {
                // Validate department
                var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
                if (department == null || department.IsDeleted)
                {
                    return ResultModel.NotFound("Department not found");
                }

                // Check for duplicate username (excluding current user if updating)
                var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
                if (existingUser != null && existingUser.Student?.StudentId != request.StudentId)
                {
                    return ResultModel.Duplicate($"Username '{request.Username}' already exists");
                }

                // Check for duplicate email (excluding current user if updating)
                var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
                if (existingEmail != null && existingEmail.Student?.StudentId != request.StudentId)
                {
                    return ResultModel.Duplicate($"Email '{request.Email}' already exists");
                }

                // Check for duplicate roll number (excluding current student if updating)
                var existingRollNumber = await _studentRepository.GetStudentByRollNumberAsync(request.RollNumber);
                if (existingRollNumber != null && existingRollNumber.StudentId != request.StudentId)
                {
                    return ResultModel.Duplicate($"Roll number '{request.RollNumber}' already exists");
                }

                // Create User entity
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = UserRole.Student,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                // Hash password only if provided (for new users or password change)
                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.PasswordHash = AuthService.HashPassword(request.Password);
                }

                // Create Student entity
                var student = new Student
                {
                    StudentId = request.StudentId ?? 0,
                    DepartmentId = request.DepartmentId,
                    RollNumber = request.RollNumber,
                    DateOfBirth = request.DateOfBirth,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    AdmissionDate = request.AdmissionDate,
                    CurrentSemester = request.CurrentSemester,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                var savedStudent = await _studentRepository.AddOrUpdateStudentAsync(student, user);

                var response = await GetStudentResponseAsync(savedStudent.StudentId);

                if (request.StudentId.HasValue && request.StudentId.Value > 0)
                {
                    return ResultModel.Updated(response, "Student updated successfully");
                }
                else
                {
                    return ResultModel.Created(response, "Student created successfully");
                }
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error adding/updating student: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetStudentByIdAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetStudentWithDetailsAsync(studentId);

                if (student == null)
                {
                    return ResultModel.NotFound("Student not found");
                }

                var response = await GetStudentResponseAsync(studentId);

                return ResultModel.Success(response, "Student retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving student: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetStudentByFilterAsync(StudentFilterModel filter)
        {
            try
            {
                var query = _studentRepository.GetQueryable()
                    .Where(s => !s.IsDeleted);

                // Apply filters
                if (!string.IsNullOrEmpty(filter.RollNumber))
                {
                    query = query.Where(s => s.RollNumber.Contains(filter.RollNumber));
                }

                if (filter.DepartmentId.HasValue)
                {
                    query = query.Where(s => s.DepartmentId == filter.DepartmentId.Value);
                }

                if (filter.CurrentSemester.HasValue)
                {
                    query = query.Where(s => s.CurrentSemester == filter.CurrentSemester.Value);
                }

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(s =>
                        s.User.FirstName.Contains(filter.SearchTerm) ||
                        s.User.LastName.Contains(filter.SearchTerm) ||
                        s.RollNumber.Contains(filter.SearchTerm));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "name" => filter.SortDescending ? query.OrderByDescending(s => s.User.FirstName) : query.OrderBy(s => s.User.FirstName),
                        "rollnumber" => filter.SortDescending ? query.OrderByDescending(s => s.RollNumber) : query.OrderBy(s => s.RollNumber),
                        _ => query.OrderBy(s => s.StudentId)
                    };
                }

                // Apply pagination
                var paginatedResult = PaginationHelper.CreatePaginationResult(
                    query,
                    filter.PageNumber,
                    filter.PageSize);

                var responses = new List<StudentResponse>();
                foreach (var student in paginatedResult.Items)
                {
                    responses.Add(await GetStudentResponseAsync(student.StudentId));
                }

                var result = new
                {
                    Items = responses,
                    paginatedResult.TotalCount,
                    paginatedResult.PageNumber,
                    paginatedResult.PageSize,
                    paginatedResult.TotalPages
                };

                return ResultModel.Success(result, "Students retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving students: {ex.Message}");
            }
        }

        public async Task<ResultModel> DeleteStudentAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetStudentWithDetailsAsync(studentId);

                if (student == null)
                {
                    return ResultModel.NotFound("Student not found");
                }

                // Soft delete User
                student.User.IsDeleted = true;
                student.User.IsActive = false;
                student.User.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(student.User);

                // Soft delete Student
                student.IsDeleted = true;
                student.UpdatedAt = DateTime.UtcNow;
                await _studentRepository.UpdateAsync(student);

                return ResultModel.Success(null, "Student deleted successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error deleting student: {ex.Message}");
            }
        }

        public async Task<ResultModel> AssignSubjectToStudentAsync(AssignSubjectToStudentRequest request)
        {
            try
            {
                // Validate student
                var student = await _studentRepository.GetByIdAsync(request.StudentId);
                if (student == null || student.IsDeleted)
                {
                    return ResultModel.NotFound("Student not found");
                }

                // Validate subject
                var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);
                if (subject == null || subject.IsDeleted)
                {
                    return ResultModel.NotFound("Subject not found");
                }

                // Check if already enrolled
                if (await _studentSubjectRepository.IsStudentEnrolledInSubjectAsync(request.StudentId, request.SubjectId, request.Semester))
                {
                    return ResultModel.Duplicate("Student is already enrolled in this subject for the specified semester");
                }

                var studentSubject = new StudentSubject
                {
                    StudentId = request.StudentId,
                    SubjectId = request.SubjectId,
                    Semester = request.Semester,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = Constants.StudentSubjectStatus.Enrolled,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                await _studentSubjectRepository.AddAsync(studentSubject);

                return ResultModel.Created(null, "Subject assigned to student successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error assigning subject to student: {ex.Message}");
            }
        }

        public async Task<ResultModel> RemoveSubjectFromStudentAsync(int studentId, int subjectId)
        {
            try
            {
                var studentSubjects = await _studentSubjectRepository.GetSubjectsByStudentAsync(studentId);
                var studentSubject = studentSubjects.FirstOrDefault(ss => ss.SubjectId == subjectId && !ss.IsDeleted);

                if (studentSubject == null)
                {
                    return ResultModel.NotFound("Student subject enrollment not found");
                }

                studentSubject.IsDeleted = true;
                studentSubject.UpdatedAt = DateTime.UtcNow;

                await _studentSubjectRepository.UpdateAsync(studentSubject);

                return ResultModel.Success(null, "Subject removed from student successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error removing subject from student: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetStudentSummaryAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetStudentWithDetailsAsync(studentId);

                if (student == null)
                {
                    return ResultModel.NotFound("Student not found");
                }

                var marks = await _markRepository.GetMarksByStudentAsync(studentId);
                var avgPercentage = marks.Any()
                    ? marks.Average(m => (m.ObtainedMarks / m.TotalMarks) * 100)
                    : (decimal?)null;

                var response = new StudentSummaryResponse
                {
                    StudentId = student.StudentId,
                    RollNumber = student.RollNumber,
                    FullName = $"{student.User.FirstName} {student.User.LastName}",
                    Email = student.User.Email,
                    DepartmentName = student.Department.DepartmentName,
                    CurrentSemester = student.CurrentSemester,
                    TotalSubjectsEnrolled = student.StudentSubjects.Count(ss => !ss.IsDeleted && ss.Status == Constants.StudentSubjectStatus.Enrolled),
                    SubjectNames = student.StudentSubjects
                        .Where(ss => !ss.IsDeleted && ss.Status == Constants.StudentSubjectStatus.Enrolled)
                        .Select(ss => ss.Subject.SubjectName)
                        .ToList(),
                    AveragePercentage = avgPercentage,
                    OverallGrade = avgPercentage.HasValue ? CalculateGrade(avgPercentage.Value) : null,
                    AdmissionDate = student.AdmissionDate,
                    IsActive = student.User.IsActive
                };

                return ResultModel.Success(response, "Student summary retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving student summary: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetStudentDetailedReportAsync(int studentId)
        {
            try
            {
                var student = await _studentRepository.GetStudentWithDetailsAsync(studentId);

                if (student == null)
                {
                    return ResultModel.NotFound("Student not found");
                }

                var marks = await _markRepository.GetMarksByStudentAsync(studentId);

                var subjectPerformances = new List<SubjectPerformance>();

                foreach (var ss in student.StudentSubjects.Where(ss => !ss.IsDeleted))
                {
                    var subjectMarks = marks.Where(m => m.SubjectId == ss.SubjectId).ToList();

                    var examPerformances = subjectMarks.Select(m => new ExamPerformance
                    {
                        ExamType = m.ExamType,
                        ObtainedMarks = m.ObtainedMarks,
                        TotalMarks = m.TotalMarks,
                        Percentage = (m.ObtainedMarks / m.TotalMarks) * 100,
                        Grade = m.Grade,
                        ExamDate = m.ExamDate
                    }).ToList();

                    var avgPercentage = examPerformances.Any()
                        ? examPerformances.Average(e => e.Percentage)
                        : (decimal?)null;

                    subjectPerformances.Add(new SubjectPerformance
                    {
                        SubjectName = ss.Subject.SubjectName,
                        SubjectCode = ss.Subject.SubjectCode,
                        Semester = ss.Semester,
                        Exams = examPerformances,
                        AveragePercentage = avgPercentage,
                        Grade = avgPercentage.HasValue ? CalculateGrade(avgPercentage.Value) : null,
                        IsPassed = avgPercentage.HasValue && avgPercentage.Value >= ss.Subject.PassingMarks
                    });
                }

                var overallPercentage = subjectPerformances.Any(sp => sp.AveragePercentage.HasValue)
                    ? subjectPerformances.Where(sp => sp.AveragePercentage.HasValue).Average(sp => sp.AveragePercentage!.Value)
                    : 0;

                var response = new StudentDetailedReportResponse
                {
                    StudentId = student.StudentId,
                    RollNumber = student.RollNumber,
                    FullName = $"{student.User.FirstName} {student.User.LastName}",
                    Email = student.User.Email,
                    DepartmentName = student.Department.DepartmentName,
                    CurrentSemester = student.CurrentSemester,
                    SubjectPerformances = subjectPerformances,
                    OverallPercentage = overallPercentage,
                    OverallGrade = CalculateGrade(overallPercentage),
                    TotalSubjects = subjectPerformances.Count,
                    PassedSubjects = subjectPerformances.Count(sp => sp.IsPassed),
                    FailedSubjects = subjectPerformances.Count(sp => !sp.IsPassed)
                };

                return ResultModel.Success(response, "Student detailed report retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving student detailed report: {ex.Message}");
            }
        }

        private async Task<StudentResponse> GetStudentResponseAsync(int studentId)
        {
            var student = await _studentRepository.GetStudentWithDetailsAsync(studentId);

            var response = new StudentResponse
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
                CreatedAt = student.CreatedAt
            };

            response.EnrolledSubjects = student.StudentSubjects
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
                .ToList();

            return response;
        }

        private string CalculateGrade(decimal percentage)
        {
            if (percentage >= 90) return Constants.Grades.APlus;
            if (percentage >= 80) return Constants.Grades.A;
            if (percentage >= 70) return Constants.Grades.BPlus;
            if (percentage >= 60) return Constants.Grades.B;
            if (percentage >= 50) return Constants.Grades.CPlus;
            if (percentage >= 40) return Constants.Grades.C;
            if (percentage >= 33) return Constants.Grades.D;
            return Constants.Grades.F;
        }
    }
}