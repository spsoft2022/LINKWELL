namespace LinkwellProductionSystem.Models
{

    public class ModelStationMap
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public string StationCode { get; set; }

        public Station Station { get; set; }
    }

}

