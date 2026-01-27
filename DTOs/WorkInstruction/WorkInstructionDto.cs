namespace LinkwellProductionSystem.DTOs.WorkInstruction
{
    public class WorkInstructionDto
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public int StationId { get; set; }
        public string HtmlContent { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public int? VersionNo { get; set; }

    }


}
