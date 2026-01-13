namespace LinkwellProductionSystem.DTOs.WorkInstruction
{
    public class WorkInstructionPreviewItemDto
    {
        public int InstructionId { get; set; }
        public string InstructionType { get; set; }   // Text / Image / PDF / Video
        public string InstructionText { get; set; }   // for Text
        public string AttachmentPath { get; set; }    // for Image / PDF / Video
        public bool IsMandatory { get; set; }
        public string ConditionJson { get; set; }
        public string ValidationJson { get; set; }
        public int VersionNo { get; set; }
        public string Status { get; set; }
    }
}
