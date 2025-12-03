
namespace SRP.Model.DTOs.Requests
{
    public class DepartmentFilterModel : FilterModel
    {
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public int? DepartmentType { get; set; }
    }
}
