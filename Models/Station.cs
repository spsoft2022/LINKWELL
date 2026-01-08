// Models/Station.cs
namespace LinkwellProductionSystem.Models
{
    public class Station
    {
        public string Id { get; set; }
        public string StationCode { get; set; } = string.Empty;   // e.g. ASSY01
        public string StationName { get; set; } = string.Empty;  // e.g. Assembly Line 01
        public string? Location { get; set; }                    // e.g. Plant A
        public bool IsActive { get; set; } = true;
    }
}