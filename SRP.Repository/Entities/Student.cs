using SRP.Repository.Entities.BaseEntities;

namespace SRP.Repository.Entities
{
    public class Student : BaseEntity
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime AdmissionDate { get; set; }
        public int CurrentSemester { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Department Department { get; set; } = null!;
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
    }


}
