using SRP.Repository.Entities.BaseEntities;

namespace SRP.Repository.Entities
{
    public class Subject : BaseEntity
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Semester { get; set; }
        public int DepartmentId { get; set; }
        public string? Description { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }

        // Navigation properties
        public Department Department { get; set; } = null!;
        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
    }
}