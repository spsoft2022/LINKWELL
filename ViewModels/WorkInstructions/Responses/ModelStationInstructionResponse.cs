namespace LinkwellProductionSystem.ViewModels.WorkInstructions.Responses
{
    public class ModelStationInstructionResponse
    {
        public int Id { get; set; }
        public int? SequenceNo { get; set; }
        public string Instruction { get; set; }
        public bool? IsMandatory { get; set; }
        public string Status { get; set; }
    }
}
