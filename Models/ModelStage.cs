public class ModelStage
{
    public int ModelId { get; set; }
    public LinkwellProductionSystem.Models.Model Model { get; set; } = null!;
    public int StageId { get; set; }
    public Stage Stage { get; set; } = null!;
    public int SequenceNo { get; set; }
}