// Models/AppUser.cs
namespace LinkwellProductionSystem.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;   // BCrypt hashed
        public string? FullName { get; set; }

        public string? Email { get; set; }
        public string? StationId { get; set; }                        // NULL for Admin
        public string Role { get; set; } = "Incharge";

        public bool MustChangePassword { get; set; }
        
        public Station? Station { get; set; }
    }
}