using System;

namespace AirportParking
{
    public interface IDateTime
    {
        DateTime UtcNow { get; }
    }

    public class AirportDateTime : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}