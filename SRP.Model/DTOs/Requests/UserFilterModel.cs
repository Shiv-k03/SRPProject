namespace SRP.Model.DTOs.Requests
{
    public class UserFilterModel : FilterModel
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public int? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
