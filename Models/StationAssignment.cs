namespace LinkwellProductionSystem.Models
{
    public class StationAssignment
    {
        public int Id { get; set; }
        public int  StationId { get; set; }
        public int ModelId { get; set; }
        public DateTime AssignedAt { get; set; }
        public string AssignedBy { get; set; }
    }

}
