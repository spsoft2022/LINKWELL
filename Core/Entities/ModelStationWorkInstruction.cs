namespace LinkwellProductionSystem.Core.Entities
{
    public class ModelStationWorkInstruction
    {
        public int Id { get; set; }

        public int? ModelId { get; set; }

        public int? StationId { get; set; }

        public int? WorkInstructionId { get; set; }

        public int? SequenceNo { get; set; }

        public bool? IsMandatory { get; set; }

        public string ConditionJson { get; set; }

        public string ValidationJson { get; set; }

        public int? VersionNo { get; set; }

        public string Status { get; set; }

        // Optional navigation properties (add later if needed)
        // public Model Model { get; set; }
        // public Station Station { get; set; }
        // public WorkInstruction WorkInstruction { get; set; }
    }
}
