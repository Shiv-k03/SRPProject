using SRP.Repository.Enums;

namespace SRP.Model.DTOs.Requests
{
    public class DepartmentRequest
    {
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public DepartmentType DepartmentType { get; set; }
        public string? Description { get; set; }
        public int? HeadOfDepartmentId { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
