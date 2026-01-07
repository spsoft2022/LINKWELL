using System.ComponentModel.DataAnnotations;

namespace LinkwellProductionSystem.ViewModels
{
    public class StationUpsertVM
    {
        public int? StationId { get; set; }   // NULL = insert, VALUE = update

        [Required]
        public string StationCode { get; set; }

        [Required]
        public string StationName { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }
    }
}
