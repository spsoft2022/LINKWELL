namespace LinkwellProductionSystem.ViewModels.WorkInstructions.Requests
{
    public class UpdateWorkInstructionRequest
    {
        // ======================
        // Mapping table
        // ======================
        public int ModelStationWorkInstructionId { get; set; }
        public int SequenceNo { get; set; }

        public int VersionNo { get; set; }
        public bool IsMandatory { get; set; }
        public string Status { get; set; }

        // ======================
        // Master table
        // ======================
        public string Title { get; set; }
        public string InstructionType { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }

        // ======================
        // Audit
        // ======================
        public string ModifiedBy { get; set; }
    }

}
