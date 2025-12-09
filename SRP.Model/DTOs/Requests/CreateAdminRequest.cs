namespace SRP.Model.DTOs.Requests
{
    public class CreateAdminRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string FirstName { get; set; } = "System";
        public string LastName { get; set; } = "Admin";
    }
}
