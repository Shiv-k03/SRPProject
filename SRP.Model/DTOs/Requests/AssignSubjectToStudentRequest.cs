namespace SRP.Model.DTOs.Requests
{
    public class AssignSubjectToStudentRequest
    {
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int Semester { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}