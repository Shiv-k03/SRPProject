namespace SRP.Model.DTOs.Responses
{
    public class StudentSummaryResponseDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int CurrentSemester { get; set; }
        public int TotalSubjectsEnrolled { get; set; }
        public int TotalSubjectsCompleted { get; set; }
        public decimal OverallPercentage { get; set; }
        public string OverallGrade { get; set; } = string.Empty;
        public List<MarkResponse> Marks { get; set; } = new();
        public List<SubjectResponseDto> EnrolledSubjects { get; set; } = new();
    }
}