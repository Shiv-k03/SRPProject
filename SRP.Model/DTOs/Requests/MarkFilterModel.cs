
namespace SRP.Model.DTOs.Requests
{
    public class MarkFilterModel : FilterModel
    {
        public int? StudentId { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }
        public string? ExamType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
