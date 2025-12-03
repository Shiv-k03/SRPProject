namespace SRP.Model.DTOs.Requests
{
    public class AssignSubjectToTeacherRequest
    {
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public int Semester { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}