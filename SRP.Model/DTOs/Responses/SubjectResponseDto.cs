namespace SRP.Model.DTOs.Responses
{
    public class SubjectResponseDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Semester { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}