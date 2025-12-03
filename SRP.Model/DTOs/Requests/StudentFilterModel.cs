using SRP.Model.DTOs.Requests;

namespace SRP.Model.DTOs.Request
{
    public class StudentFilterModel : FilterModel
    {
        public string? RollNumber { get; set; }
        public int? DepartmentId { get; set; }
        public int? CurrentSemester { get; set; }
    }
}