using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRP.Model.DTOs.Responses
{
    public class DashboardResponse
    {
        public int TotalDepartments { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSubjects { get; set; }
        public List<DepartmentStatistics> DepartmentStats { get; set; } = new();
    }

    public class DepartmentStatistics
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalTeachers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSubjects { get; set; }
    }
}
