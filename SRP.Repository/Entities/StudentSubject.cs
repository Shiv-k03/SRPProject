using SRP.Repository.Entities.BaseEntities;

namespace SRP.Repository.Entities
{
    public class StudentSubject : BaseEntity
    {
        public int StudentSubjectId { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int Semester { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Completed, Dropped

        // Navigation properties
        public Student Student { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}