using SRP.Business.Interfaces;
using SRP.Model.DTOs.Request;
using SRP.Model.DTOs.Requests;
using SRP.Model.DTOs.Responses;
using SRP.Repository.Entities;
using SRP.Repository.Interfaces;
using SRP.Model.Helper.Base;
using SRP.Model.Helper.Helpers;

namespace SRP.Business.Services
{
    public class MarkService : IMarkService
    {
        private readonly IMarkRepository _markRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentSubjectRepository _studentSubjectRepository;

        public MarkService(
            IMarkRepository markRepository,
            IStudentRepository studentRepository,
            ISubjectRepository subjectRepository,
            ITeacherRepository teacherRepository,
            IStudentSubjectRepository studentSubjectRepository)
        {
            _markRepository = markRepository;
            _studentRepository = studentRepository;
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
            _studentSubjectRepository = studentSubjectRepository;
        }

        public async Task<ResultModel> AddOrUpdateMarkAsync(MarkRequest request)
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

                // Validate teacher
                var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);
                if (teacher == null || teacher.IsDeleted)
                {
                    return ResultModel.NotFound("Teacher not found");
                }

                // Check if student is enrolled in the subject
                var studentSubjects = await _studentSubjectRepository.GetSubjectsByStudentAsync(request.StudentId);
                var enrollment = studentSubjects.FirstOrDefault(ss => ss.SubjectId == request.SubjectId && !ss.IsDeleted);
                if (enrollment == null)
                {
                    return ResultModel.Invalid("Student is not enrolled in this subject");
                }

                // Validate marks
                if (request.ObtainedMarks > request.TotalMarks)
                {
                    return ResultModel.Invalid("Obtained marks cannot exceed total marks");
                }

                var mark = new Mark
                {
                    MarkId = request.MarkId ?? 0,
                    StudentId = request.StudentId,
                    SubjectId = request.SubjectId,
                    TeacherId = request.TeacherId,
                    ObtainedMarks = request.ObtainedMarks,
                    TotalMarks = request.TotalMarks,
                    ExamType = request.ExamType,
                    ExamDate = request.ExamDate,
                    Remarks = request.Remarks,
                    Grade = CalculateGrade(request.ObtainedMarks, request.TotalMarks),
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.UpdatedBy
                };

                var savedMark = await _markRepository.AddOrUpdateMarkAsync(mark);

                var response = await GetMarkResponseAsync(savedMark.MarkId);

                if (request.MarkId.HasValue && request.MarkId.Value > 0)
                {
                    return ResultModel.Updated(response, "Marks updated successfully");
                }
                else
                {
                    return ResultModel.Created(response, "Marks added successfully");
                }
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error adding/updating marks: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetMarkByIdAsync(int markId)
        {
            try
            {
                var mark = await _markRepository.GetMarkWithDetailsAsync(markId);

                if (mark == null)
                {
                    return ResultModel.NotFound("Mark not found");
                }

                var response = await GetMarkResponseAsync(markId);

                return ResultModel.Success(response, "Mark retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving mark: {ex.Message}");
            }
        }

        public async Task<ResultModel> GetMarkByFilterAsync(MarkFilterModel filter)
        {
            try
            {
                var query = _markRepository.GetQueryable()
                    .Where(m => !m.IsDeleted);

                // Apply filters
                if (filter.StudentId.HasValue)
                {
                    query = query.Where(m => m.StudentId == filter.StudentId.Value);
                }

                if (filter.SubjectId.HasValue)
                {
                    query = query.Where(m => m.SubjectId == filter.SubjectId.Value);
                }

                if (filter.TeacherId.HasValue)
                {
                    query = query.Where(m => m.TeacherId == filter.TeacherId.Value);
                }

                if (!string.IsNullOrEmpty(filter.ExamType))
                {
                    query = query.Where(m => m.ExamType.Contains(filter.ExamType));
                }

                if (filter.FromDate.HasValue)
                {
                    query = query.Where(m => m.ExamDate >= filter.FromDate.Value);
                }

                if (filter.ToDate.HasValue)
                {
                    query = query.Where(m => m.ExamDate <= filter.ToDate.Value);
                }

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    query = query.Where(m =>
                        m.ExamType.Contains(filter.SearchTerm) ||
                        m.Grade.Contains(filter.SearchTerm));
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    query = filter.SortBy.ToLower() switch
                    {
                        "examdate" => filter.SortDescending ? query.OrderByDescending(m => m.ExamDate) : query.OrderBy(m => m.ExamDate),
                        "marks" => filter.SortDescending ? query.OrderByDescending(m => m.ObtainedMarks) : query.OrderBy(m => m.ObtainedMarks),
                        _ => query.OrderBy(m => m.MarkId)
                    };
                }

                // Apply pagination
                var paginatedResult = PaginationHelper.CreatePaginationResult(
                    query,
                    filter.PageNumber,
                    filter.PageSize);

                var responses = new List<MarkResponse>();
                foreach (var mark in paginatedResult.Items)
                {
                    responses.Add(await GetMarkResponseAsync(mark.MarkId));
                }

                var result = new
                {
                    Items = responses,
                    paginatedResult.TotalCount,
                    paginatedResult.PageNumber,
                    paginatedResult.PageSize,
                    paginatedResult.TotalPages
                };

                return ResultModel.Success(result, "Marks retrieved successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error retrieving marks: {ex.Message}");
            }
        }

        public async Task<ResultModel> DeleteMarkAsync(int markId)
        {
            try
            {
                var mark = await _markRepository.GetByIdAsync(markId);

                if (mark == null || mark.IsDeleted)
                {
                    return ResultModel.NotFound("Mark not found");
                }

                mark.IsDeleted = true;
                mark.UpdatedAt = DateTime.UtcNow;

                await _markRepository.UpdateAsync(mark);

                return ResultModel.Success(null, "Mark deleted successfully");
            }
            catch (Exception ex)
            {
                return ResultModel.Exception($"Error deleting mark: {ex.Message}");
            }
        }

        private async Task<MarkResponse> GetMarkResponseAsync(int markId)
        {
            var mark = await _markRepository.GetMarkWithDetailsAsync(markId);

            var percentage = (mark.ObtainedMarks / mark.TotalMarks) * 100;

            return new MarkResponse
            {
                MarkId = mark.MarkId,
                StudentId = mark.StudentId,
                StudentName = $"{mark.Student.User.FirstName} {mark.Student.User.LastName}",
                RollNumber = mark.Student.RollNumber,
                SubjectId = mark.SubjectId,
                SubjectName = mark.Subject.SubjectName,
                SubjectCode = mark.Subject.SubjectCode,
                TeacherId = mark.TeacherId,
                ObtainedMarks = mark.ObtainedMarks,
                TotalMarks = mark.TotalMarks,
                Percentage = percentage,
                ExamType = mark.ExamType,
                ExamDate = mark.ExamDate,
                Remarks = mark.Remarks,
                Grade = mark.Grade,
                CreatedAt = mark.CreatedAt
            };
        }

        private string CalculateGrade(decimal obtainedMarks, decimal totalMarks)
        {
            var percentage = (obtainedMarks / totalMarks) * 100;

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