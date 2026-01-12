using System.ComponentModel.DataAnnotations.Schema;

namespace LinkwellProductionSystem.Models
{

    public class ModelStationMap
    {
        public int ModelId { get; set; }
        public int StationId { get; set; }   // non-nullable
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }





}

