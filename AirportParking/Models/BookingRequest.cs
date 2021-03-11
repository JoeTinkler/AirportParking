namespace AirportParking.Models
{
    public class BookingRequest
    {
        public int ParkingSpaceId { get; set; }
        public string AircraftTypeId { get; set; }
        public int Days { get; set; } = 0;
        public int Hours { get; set; } = 0;
    }
}