namespace LinkwellProductionSystem.ViewModels.WorkInstructions
{
    public class SaveModelStationWorkInstructionVM
    {
        public ModelStationContextVM Context { get; set; }

        public List<ModelStationWorkInstructionVM> Instructions { get; set; }

        public bool Publish { get; set; }
    }
}
