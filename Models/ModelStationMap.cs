namespace LinkwellProductionSystem.Models
{
    public class ModelStationMap
    {
        public int Id { get; set; }

        public int ModelId { get; set; }
        public int StationId { get; set; }

        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        // OPTIONAL navigation properties
        public Model Model { get; set; }   // or MachineModel
        public Station Station { get; set; }
    }
}
