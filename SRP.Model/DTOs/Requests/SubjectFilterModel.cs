namespace SRP.Model.DTOs.Requests
{
    public class SubjectFilterModel :  FilterModel
    {
        public string? SubjectCode { get; set; }
        public string? DepartmentId { get; set; }
        public int? Semester { get; set; }
    }
}
