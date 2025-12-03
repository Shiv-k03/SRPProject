namespace SRP.Model.DTOs.Requests
{
    public class SubjectRequest
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Semester { get; set; }
        public int DepartmentId { get; set; }
        public string? Description { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
