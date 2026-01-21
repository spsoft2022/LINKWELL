namespace LinkwellProductionSystem.Core.Entities
{
    public class WorkInstruction
    {
        public int? Id { get; set; }
        public string HtmlContent { get; set; }
        public int ModelId { get; set; }
        public int StationId { get; set; }
        public string Status { get; set; }
        public int? VersionNo { get; set; }
        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public bool IsActive { get; set; }
        

    }

}
