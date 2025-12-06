using SRP.Repository.Enums;

namespace SRP.Model.DTOs.Responses
{
    public class UserProfileResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }

        // Additional profile info based on role
        public TeacherResponse? TeacherProfile { get; set; }
        public StudentResponse? StudentProfile { get; set; }
    }
}
