namespace LinkwellProductionSystem.ViewModels.WorkInstructions.Requests
{
    public class AddWorkInstructionRequest
    {
        // WorkInstruction (MASTER)
        public string Title { get; set; }
        public string InstructionType { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }

        // Mapping
        public int ModelId { get; set; }
        public int StationId { get; set; }
        public int SequenceNo { get; set; }
        public bool IsMandatory { get; set; }
        public int VersionNo { get; set; }
        public string Status { get; set; }

        public string CreatedBy { get; set; }
    }

}
