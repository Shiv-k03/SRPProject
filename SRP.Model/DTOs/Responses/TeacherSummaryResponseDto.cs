namespace SRP.Model.DTOs.Responses
{
    public class TeacherSummaryResponseDto
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int TotalSubjectsAssigned { get; set; }
        public int TotalStudentsTeaching { get; set; }
        public List<SubjectResponseDto> AssignedSubjects { get; set; } = new();
    }
}