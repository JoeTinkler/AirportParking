using System.Collections.Generic;

namespace AirportParking.Models
{
    public class ParkingSpace
    {
        public int Id { get; set; }
        public AircraftSize Size { get; set; }
        public List<BookingAppointment> Appointments { get; set; } = new List<BookingAppointment>();
    }
}