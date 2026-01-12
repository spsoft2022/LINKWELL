namespace LinkwellProductionSystem.ViewModels.WorkInstructions.Content
{
    public class ParameterItemVM
    {
        public string Name { get; set; }
        public decimal ExpectedValue { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public string Unit { get; set; }
    }
    public class ParameterInstructionContentVM
    {
        public List<ParameterItemVM> Parameters { get; set; }
    }
}
