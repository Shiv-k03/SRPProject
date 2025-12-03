
namespace SRP.Model.DTOs.Requests
{
    public class TeacherFilterModel : FilterModel
    {
        public string? EmployeeCode { get; set; }
        public int? DepartmentId { get; set; }
        public int? Role { get; set; }
    }
}
