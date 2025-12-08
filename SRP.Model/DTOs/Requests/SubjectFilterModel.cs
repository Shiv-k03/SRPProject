namespace SRP.Model.DTOs.Requests
{
    public class SubjectFilterModel :  FilterModel
    {
        public string? SubjectCode { get; set; }
        public int? DepartmentId { get; set; }
        public int? Semester { get; set; }
    }
}
