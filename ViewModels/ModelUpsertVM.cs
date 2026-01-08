using System.ComponentModel.DataAnnotations;

namespace LinkwellProductionSystem.ViewModels
{
    public class ModelUpsertVM
    {
        public int? Id { get; set; }   // NULL = Insert, VALUE = Update

        [Required]
        [StringLength(50)]
        public string ModelCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ModelName { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryId { get; set; }


    }
}
