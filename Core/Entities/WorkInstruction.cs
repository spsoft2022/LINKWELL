namespace LinkwellProductionSystem.Core.Entities
{
    public class WorkInstruction
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? InstructionType { get; set; }  // Text / Image / PDF etc.

        public string? Content { get; set; }          // Actual instruction / JSON

        public bool? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string? ModifiedBy { get; set; }
    }

}
