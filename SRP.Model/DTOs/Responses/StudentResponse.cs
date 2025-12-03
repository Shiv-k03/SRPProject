namespace SRP.Model.DTOs.Responses
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty ;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string RollNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime AdmissionDate { get; set; }
        public int CurrentSemester { get; set; }
        public bool IsActive { get; set; }
        public List<SubjectResponseDto> EnrolledSubjects { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}