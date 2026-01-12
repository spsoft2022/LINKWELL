using LinkwellProductionSystem.Core.Enums;

namespace LinkwellProductionSystem.ViewModels.WorkInstructions
{
    public class ModelStationInstructionListVM
    {
        public int Id { get; set; }
        public int SequenceNo { get; set; }
        public string Title { get; set; }
        public WorkInstructionType InstructionType { get; set; }
        public bool IsMandatory { get; set; }
        public InstructionStatus Status { get; set; }
    }

}
