using System.Collections.Generic;
using AirportParking.Models;

namespace AirportParking
{
    public interface IDataHelper
    {
        IEnumerable<AircraftType> GetAircraftTypes();
        AircraftType GetAircraftType(string id);
        void AddOrUpdateAircraftType(AircraftType type);
        void RemoveAircraftType(string id);
        IEnumerable<ParkingSpace> GetParkingSpaces();
        ParkingSpace GetParkingSpace(int id);
        void AddOrUpdateParkingSpace(ParkingSpace space);
        void RemoveParkingSpace(int id);
    }
}