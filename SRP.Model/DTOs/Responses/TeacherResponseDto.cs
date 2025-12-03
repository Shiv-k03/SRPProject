using SRP.Repository.Enums;

namespace SRP.Model.DTOs.Responses
{
    public class TeacherResponseDto
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool IsActive { get; set; }
        public List<SubjectResponseDto> AssignedSubjects { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}