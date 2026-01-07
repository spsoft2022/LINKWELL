// Models/WorkInstruction.cs
namespace LinkwellProductionSystem.Models
{
    public class WorkInstruction
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public int StageId { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public string? AttachmentPath { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation properties (optional but helpful)
        public Model Model { get; set; } = null!;
        public Stage Stage { get; set; } = null!;
    }
}