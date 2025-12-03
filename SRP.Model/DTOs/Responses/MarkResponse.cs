namespace SRP.Model.DTOs.Responses
{
    public class MarkResponse
    {
        public int MarkId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int TeacherId { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Percentage { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string? Remarks { get; set; }
        public string Grade { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}