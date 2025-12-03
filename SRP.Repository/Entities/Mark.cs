using SRP.Repository.Entities.BaseEntities;

namespace SRP.Repository.Entities
{
    public class Mark : BaseEntity
    {
        public int MarkId { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal TotalMarks { get; set; }
        public string ExamType { get; set; } = string.Empty; // Midterm, Final, Assignment
        public DateTime ExamDate { get; set; }
        public string? Remarks { get; set; }
        public string Grade { get; set; } = string.Empty;

        // Navigation properties
        public Student Student { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}