using LinkwellProductionSystem.Core.Enums;

namespace LinkwellProductionSystem.ViewModels.WorkInstructions
{
    public class OperatorWorkInstructionVM
    {
        public int SequenceNo { get; set; }
        public string Title { get; set; }
        public WorkInstructionType InstructionType { get; set; }
        public string Content { get; set; }
        public string ValidationJson { get; set; }
        public bool IsMandatory { get; set; }
    }
}
