using Newtonsoft.Json;

namespace AirportParking.Models
{
    public class AircraftType
    {
        [JsonRequired]
        public string Id { get; set; }
        public AircraftSize Size { get; set; }
    }
}