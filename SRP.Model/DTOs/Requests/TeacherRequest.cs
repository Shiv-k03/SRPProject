using SRP.Repository.Enums;

namespace SRP.Model.DTOs.Requests
{
    public class TeacherRequest
    {
        public int? TeacherId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int DepartmentId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime JoiningDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}