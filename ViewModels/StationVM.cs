using Microsoft.EntityFrameworkCore;

namespace LinkwellProductionSystem.ViewModels
{
    [Keyless]
    public class StationVM
    {
        public int Id { get; set; }
        public string StationCode { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;

        public string? Description { get; set; }   // ✅ nullable
        public string? Location { get; set; }      // ✅ nullable
        public bool? IsActive { get; set; }         // ✅ nullable
    }

}
