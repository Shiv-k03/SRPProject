using SRP.Repository.Entities.BaseEntities;

namespace SRP.Repository.Entities
{
    public class TeacherSubject : BaseEntity
    {
        public int TeacherSubjectId { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public int Semester { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }

        // Navigation properties
        public Teacher Teacher { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}