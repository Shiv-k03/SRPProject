using SRP.Repository.Entities.BaseEntities;

namespace SRP.Repository.Entities
{
    public class Teacher : BaseEntity
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime JoiningDate { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Department Department { get; set; } = null!;
        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    }


}
