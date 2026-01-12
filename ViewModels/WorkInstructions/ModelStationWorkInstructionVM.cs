using LinkwellProductionSystem.Core.Enums;

namespace LinkwellProductionSystem.ViewModels.WorkInstructions
{
    public class ModelStationWorkInstructionVM
    {
        public int Id { get; set; }

        public int ModelId { get; set; }
        public int StationId { get; set; }

        public int WorkInstructionId { get; set; }
        public string WorkInstructionTitle { get; set; }

        public WorkInstructionType InstructionType { get; set; }

        public int SequenceNo { get; set; }

        public bool IsMandatory { get; set; }

        /// <summary>
        /// JSON condition (meter type, phase, CT/LT)
        /// </summary>
        public string ConditionJson { get; set; }

        /// <summary>
        /// JSON validation rules (range, ack, proof)
        /// </summary>
        public string ValidationJson { get; set; }

        public int VersionNo { get; set; }

        public InstructionStatus Status { get; set; }
    }

}
