// Models/HourlyProduction.cs
namespace LinkwellProductionSystem.Models
{
    public class HourlyProduction
    {
        public int Id { get; set; }
        public int DailyProductionId { get; set; }
        public string HourSlot { get; set; } = string.Empty;   // "09-10", "10-11", etc.
        public int? Planned { get; set; }
        public int? Actual { get; set; }
        public string? DowntimeReason { get; set; }

        public DailyProduction DailyProduction { get; set; } = null!;
    }
}