namespace SRP.Model.DTOs.Responses
{
    public class StudentSummaryResponse
    {
        public int StudentId { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int CurrentSemester { get; set; }
        public int TotalSubjectsEnrolled { get; set; }
        public List<string> SubjectNames { get; set; } = new();
        public decimal? AveragePercentage { get; set; }
        public string? OverallGrade { get; set; }
        public DateTime AdmissionDate { get; set; }
        public bool IsActive { get; set; }
    }
}