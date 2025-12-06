using SRP.Repository.Entities.BaseEntities;
using SRP.Repository.Enums;

namespace SRP.Repository.Entities
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        
        // Navigation properties
        public Teacher? Teacher { get; set; }
        public Student? Student { get; set; }
        public DateTime LastLogin { get; set; }
    }
}