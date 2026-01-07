// Models/DailyProduction.cs
namespace LinkwellProductionSystem.Models
{
    public class DailyProduction
    {
        public int Id { get; set; }
        public int StationId { get; set; }
        public DateTime ProductionDate { get; set; } = DateTime.Today;
        public int DailyTarget { get; set; } = 0;

        public Station Station { get; set; } = null!;
        public ICollection<HourlyProduction> HourlyProductions { get; set; } = new List<HourlyProduction>();
    }
}