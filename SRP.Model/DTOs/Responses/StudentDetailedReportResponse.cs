using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRP.Model.DTOs.Responses
{
    public class StudentDetailedReportResponse
    {
        public int StudentId { get; set; }
        public string RollNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int CurrentSemester { get; set; }
        public List<SubjectPerformance> SubjectPerformances { get; set; } = new();
        public decimal OverallPercentage { get; set; }
        public string OverallGrade { get; set; } = string.Empty;
        public int TotalSubjects { get; set; }
        public int PassedSubjects { get; set; }
        public int FailedSubjects { get; set; }
    }

    public class SubjectPerformance
    {
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int Semester { get; set; }
        public List<ExamPerformance> Exams { get; set; } = new();
        public decimal? AveragePercentage { get; set; }
        public string? Grade { get; set; }
        public bool IsPassed { get; set; }
    }

    public class ExamPerformance
    {
        public string ExamType { get; set; } = string.Empty;
        public decimal ObtainedMarks { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
    }
}
