using SRP.Repository.Enums;

namespace SRP.Model.DTOs.Responses
{
    public class TeacherSummaryResponse
    {
        public int TeacherId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public int TotalSubjectsAssigned { get; set; }
        public List<string> SubjectNames { get; set; } = new();
        public DateTime JoiningDate { get; set; }
        public bool IsActive { get; set; }
    }
}