namespace LinkwellProductionSystem.Models
{
    public class WorkInstructions
    {
        public int Id { get; set; }
        public string HtmlContent { get; set; }
        public int ModelId { get; set; }
        public int StationId { get; set; }
        public string Status { get; set; }
        public int VersionNo { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
    }
}
