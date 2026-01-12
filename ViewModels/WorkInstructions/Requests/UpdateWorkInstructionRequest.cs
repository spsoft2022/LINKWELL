namespace LinkwellProductionSystem.ViewModels.WorkInstructions.Requests
{
    public class UpdateWorkInstructionRequest
    {
        public int ModelStationWorkInstructionId { get; set; }
        public string Instruction { get; set; }
        public string? AttachmentPath { get; set; }
        public int SequenceNo { get; set; }
        public bool IsMandatory { get; set; }
        public string Status { get; set; }
    }
}
