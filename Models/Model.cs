namespace LinkwellProductionSystem.Models
{
    public class Model
    {
        public int Id { get; set; }
        public string ModelCode { get; set; } = null!;
        public string ModelName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}