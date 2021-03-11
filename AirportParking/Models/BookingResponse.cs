namespace AirportParking.Models
{
    public class BookingResponse
    {
        public int ParkingSpaceId { get; set; }
        public BookingAppointment Appointment { get; set; }
    }
}