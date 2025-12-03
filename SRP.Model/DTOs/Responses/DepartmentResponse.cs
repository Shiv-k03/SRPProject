using SRP.Repository.Enums;

namespace SRP.Model.DTOs.Responses
{
    public class DepartmentResponse
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public DepartmentType DepartmentType { get; set; }
        public string DepartmentTypeName => DepartmentType.ToString();
        public string? Description { get; set; }
        public int? HeadOfDepartmentId { get; set; }
        public string? HeadOfDepartmentName { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSubjects { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}