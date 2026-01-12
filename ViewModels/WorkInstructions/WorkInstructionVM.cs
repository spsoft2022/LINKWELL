using LinkwellProductionSystem.Core.Enums;

namespace LinkwellProductionSystem.ViewModels.WorkInstructions
{
    public class WorkInstructionVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public WorkInstructionType InstructionType { get; set; }

        /// <summary>
        /// HTML / JSON / URL based on InstructionType
        /// </summary>
        public string Content { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }

}
