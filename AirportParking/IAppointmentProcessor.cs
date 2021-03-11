using AirportParking.Models;

namespace AirportParking
{
    public interface IAppointmentProcessor
    {
        int RecommendSpace(RecommendRequest request);
        BookingResponse BookAppointment(BookingRequest request);
    }
}