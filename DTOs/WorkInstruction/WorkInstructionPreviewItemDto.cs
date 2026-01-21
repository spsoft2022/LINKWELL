namespace LinkwellProductionSystem.DTOs.WorkInstruction
{
    public class WorkInstructionPreviewItemDto
    {
        public int InstructionId { get; set; }
        public string htmlContent { get; set; }    // for Image / PDF / Video
        public bool IsMandatory { get; set; }
        public string Status { get; set; }
    }
}
