namespace LinkwellProductionSystem.ViewModels.WorkInstructions.Requests
{
    public class UpdateWorkInstructionRequest
    {
        public int ModelStationWorkInstructionId { get; set; }

        public string? Title { get; set; }
        public string? InstructionType { get; set; }
        public string? Content { get; set; }

        public int SequenceNo { get; set; }
        public bool IsMandatory { get; set; }
        public string Status { get; set; } = "Active";
        public string VersionNo { get; set; } = "V1";

        public string ModifiedBy { get; set; } = "UI";
        public bool IsActive { get; set; } = true;
    }


}
