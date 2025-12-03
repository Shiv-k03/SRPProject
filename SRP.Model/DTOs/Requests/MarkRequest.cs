namespace SRP.Model.DTOs.Requests
{
    public class MarkRequest
    {
        public int? MarkId { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal TotalMarks { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}