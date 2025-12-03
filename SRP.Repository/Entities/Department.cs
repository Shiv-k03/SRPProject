using SRP.Repository.Entities.BaseEntities;
using SRP.Repository.Enums;

namespace SRP.Repository.Entities
{
    public class Department : BaseEntity
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public DepartmentType DepartmentType { get; set; }
        public string? Description { get; set; }
        public int? HeadOfDepartmentId { get; set; }

        // Navigation properties
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}