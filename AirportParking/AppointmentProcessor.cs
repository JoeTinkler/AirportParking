using System;
using System.Collections.Generic;
using System.Linq;
using AirportParking.Models;

namespace AirportParking
{
    public class AppointmentProcessor : IAppointmentProcessor
    {
        private readonly IDataHelper _dataHelper;
        private readonly IDateTime _dateTime;
        
        public AppointmentProcessor(IDataHelper dataHelper, IDateTime dateTime)
        {
            _dataHelper = dataHelper;
            _dateTime = dateTime;
        }

        public int RecommendSpace(RecommendRequest request)
        {
            var appointment = CreateAppointment(request.Days, request.Hours);
            var aircraftType = _dataHelper.GetAircraftType(request.AircraftTypeId);
            if (aircraftType == null)
            {
                throw new ArgumentOutOfRangeException(nameof(request.AircraftTypeId));
            }

            var spaces = _dataHelper.GetParkingSpaces().ToList();
            var spaceSize = aircraftType.Size;
            
            // attempt smallest size first then check for space in larges parking spaces
            while (spaceSize <= AircraftSize.Jumbo)
            {
                var space = FindAvailableSpace(spaces, spaceSize, appointment);
                if (space != null)
                {
                    return space.Id;
                }

                spaceSize++;
            }

            throw new Exception("Could not book appointment, no free spaces found");
        }

        public BookingResponse BookAppointment(BookingRequest request)
        {
            var appointment = CreateAppointment(request.Days, request.Hours);
            var space = _dataHelper.GetParkingSpace(request.ParkingSpaceId);
            if (space == null)
            {
                throw new Exception($"Could not book appointment, parking space ({request.ParkingSpaceId}) not found");
            }
            
            var aircraftType = _dataHelper.GetAircraftType(request.AircraftTypeId);
            if (aircraftType == null)
            {
                throw new ArgumentOutOfRangeException(nameof(request.AircraftTypeId));
            }

            if (!ValidSpace(space, aircraftType.Size, appointment))
            {
                throw new Exception("Could not book appointment, no free spaces found");
            }
            
            space.Appointments.Add(appointment);
            _dataHelper.AddOrUpdateParkingSpace(space);
            return new BookingResponse { ParkingSpaceId = space.Id, Appointment = appointment };
        }

        private BookingAppointment CreateAppointment(int days, int hours)
        {
            return new BookingAppointment
            {
                StartDateTime = _dateTime.UtcNow,
                EndDateTime = _dateTime.UtcNow.AddDays(days).AddHours(hours),
            };
        }

        private ParkingSpace FindAvailableSpace(List<ParkingSpace> spaces, AircraftSize size, BookingAppointment appointment)
        {
            return spaces.FirstOrDefault(space => ValidSpace(space, size, appointment));
        }

        private bool ValidSpace(ParkingSpace space, AircraftSize size, BookingAppointment appointment)
        {
            return space.Size == size
                && space.Appointments.All(spaceAppointment => !AppointmentsOverlap(spaceAppointment, appointment));
        }

        private bool AppointmentsOverlap(BookingAppointment x, BookingAppointment y)
        {
            return x.StartDateTime < y.EndDateTime && y.StartDateTime < x.EndDateTime;
        }
    }
}