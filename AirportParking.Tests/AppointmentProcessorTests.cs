using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using AirportParking.Models;

namespace AirportParking.Tests
{
    [TestFixture]
    public class Tests
    {
        private DateTime DefaultDateTime => new DateTime(2021, 03, 11, 18, 0, 0);
        private IDateTime MockDateTime()
        {
            var dateTime = new Mock<IDateTime>();
            dateTime.Setup(dt => dt.UtcNow).Returns(DefaultDateTime);

            return dateTime.Object;
        }
        
        private List<AircraftType> DefaultAircraftTypes => new List<AircraftType> 
        {
            new AircraftType { Id = "A380", Size = AircraftSize.Jumbo },
            new AircraftType { Id = "B747", Size = AircraftSize.Jumbo },
            new AircraftType { Id = "A330", Size = AircraftSize.Jet },
            new AircraftType { Id = "B777", Size = AircraftSize.Jet },
            new AircraftType { Id = "E195", Size = AircraftSize.Prop },
        };

        private List<ParkingSpace> DefaultParkingSpaces => GetParkingSpaces(25, 50, 25);

        private BookingAppointment DefaultAppointment => new BookingAppointment
        {
            StartDateTime = DefaultDateTime,
            EndDateTime = DefaultDateTime.AddDays(1).AddHours(1),
        };

        private List<ParkingSpace> GetParkingSpaces(int propSpacesCount, int jetSpacesCount, int jumboSpacesCount)
        {
            var propSpaces = GetParkingSpaces(1, propSpacesCount, AircraftSize.Prop);
            var jetSpaces = GetParkingSpaces(propSpacesCount+1, jetSpacesCount, AircraftSize.Jet);
            var jumboSpaces = GetParkingSpaces(propSpacesCount+jetSpacesCount+1, jumboSpacesCount, AircraftSize.Jumbo);

            return propSpaces.Concat(jetSpaces).Concat(jumboSpaces).ToList();
        }

        private List<ParkingSpace> GetParkingSpaces(int startId, int count, AircraftSize size)
        {
            return Enumerable.Range(startId, count)
                .Select(i => new ParkingSpace {Id = i, Size = size})
                .ToList();
        }

        private Mock<IDataHelper> SetupData(List<AircraftType> aircraftTypes, List<ParkingSpace> parkingSpaces)
        {
            var dataHelper = new Mock<IDataHelper>();
            dataHelper.Setup(da => da.GetAircraftType(It.IsAny<string>()))
                .Returns((string id) => aircraftTypes.FirstOrDefault(t => t.Id == id));
            dataHelper.Setup(da => da.GetAircraftTypes())
                .Returns(aircraftTypes);
            dataHelper.Setup(da => da.GetParkingSpace(It.IsAny<int>()))
                .Returns((int id) => parkingSpaces.FirstOrDefault(t => t.Id == id));
            dataHelper.Setup(da => da.GetParkingSpaces())
                .Returns(parkingSpaces);
            
            return dataHelper;
        }
        
        [Test]
        [TestCase("A380", 76)]
        [TestCase("B747", 76)]
        [TestCase("A330", 26)]
        [TestCase("B777", 26)]
        [TestCase("E195", 1)]
        public void RecommendSpace_ReturnsExpectedSpaceId(string aircraftTypeId, int expectedParkingSpaceId)
        {
            var dataHelper = SetupData(DefaultAircraftTypes, DefaultParkingSpaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new RecommendRequest { AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            var result = processor.RecommendSpace(request);
            
            Assert.AreEqual(expectedParkingSpaceId, result);
        }

        [Test]
        public void RecommendSpace_ReturnsAvailableSpace()
        {
            var aircraftTypeId = "A380";
            var expectedParkingSpaceId = 77;
            var spaces = DefaultParkingSpaces;
            spaces.First(space => space.Id == 76).Appointments.Add(DefaultAppointment);
            
            var dataHelper = SetupData(DefaultAircraftTypes, spaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new RecommendRequest { AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            var result = processor.RecommendSpace(request);
            
            Assert.AreEqual(expectedParkingSpaceId, result);
        }
        
        [Test]
        [TestCase("A380", 1)]
        [TestCase("A330", 2)]
        [TestCase("E195", 3)]
        public void RecommendSpace_ReturnsSmallestSpace(string aircraftTypeId, int expectedParkingSpaceId)
        {
            var spaces = new List<ParkingSpace>
            {
                new ParkingSpace { Id = 1, Size = AircraftSize.Jumbo },
                new ParkingSpace { Id = 2, Size = AircraftSize.Jet },
                new ParkingSpace { Id = 3, Size = AircraftSize.Prop },
            };
            
            var dataHelper = SetupData(DefaultAircraftTypes, spaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new RecommendRequest { AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            var result = processor.RecommendSpace(request);
            
            Assert.AreEqual(expectedParkingSpaceId, result);
        }
        
        [Test]
        [TestCase("A330", 1)]
        [TestCase("E195", 1)]
        public void RecommendSpace_UsesLargerSpace(string aircraftTypeId, int expectedParkingSpaceId)
        {
            var spaces = new List<ParkingSpace>
            {
                new ParkingSpace { Id = 1, Size = AircraftSize.Jumbo },
            };
            
            var dataHelper = SetupData(DefaultAircraftTypes, spaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new RecommendRequest { AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            var result = processor.RecommendSpace(request);
            
            Assert.AreEqual(expectedParkingSpaceId, result);
        }

        [Test] [TestCase("A380")]
        [TestCase("A330")]
        public void RecommendSpace_DoesNotUseSmallerSpace(string aircraftTypeId)
        {
            var spaces = new List<ParkingSpace>
            {
                new ParkingSpace { Id = 1, Size = AircraftSize.Prop },
            };
            
            var dataHelper = SetupData(DefaultAircraftTypes, spaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new RecommendRequest { AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            Assert.Throws<Exception>(() => processor.RecommendSpace(request));
        }
        
        [Test]
        public void RecommendSpace_ThrowsErrorWhenNoSpaces()
        {
            var aircraftTypeId = "A380";
            var spaces = new List<ParkingSpace>
            {
                new ParkingSpace { Id = 1, Size = AircraftSize.Jumbo, Appointments = new List<BookingAppointment> { DefaultAppointment } },
            };

            var dataHelper = SetupData(DefaultAircraftTypes, spaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new RecommendRequest { AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            Assert.Throws<Exception>(() => processor.RecommendSpace(request));
        }

        [Test]
        public void RecommendSpace_ThrowsErrorWhenNoDuration()
        {
            var aircraftTypeId = "A380";
            var dataHelper = SetupData(DefaultAircraftTypes, DefaultParkingSpaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new RecommendRequest { AircraftTypeId = aircraftTypeId, Days = 0, Hours = 0 };
            Assert.Throws<Exception>(() => processor.RecommendSpace(request));
        }
        
        [Test]
        [TestCase("A380", 76)]
        [TestCase("B747", 76)]
        [TestCase("A330", 26)]
        [TestCase("B777", 26)]
        [TestCase("E195", 1)]
        public void BookAppointment_ReturnsExpectedResult(string aircraftTypeId, int parkingSpaceId)
        {
            var dataHelper = SetupData(DefaultAircraftTypes, DefaultParkingSpaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new BookingRequest { ParkingSpaceId = parkingSpaceId, AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            var result = processor.BookAppointment(request);
            
            Assert.AreEqual(DefaultAppointment.StartDateTime, result.Appointment.StartDateTime);
            Assert.AreEqual(DefaultAppointment.EndDateTime, result.Appointment.EndDateTime);
            Assert.AreEqual(parkingSpaceId, result.ParkingSpaceId);
        }
        
        [Test]
        public void BookAppointment_ThrowsErrorWhenSpaceIncorrectSize()
        {
            var aircraftTypeId = "A380";
            var spaces = new List<ParkingSpace>
            {
                new ParkingSpace { Id = 1, Size = AircraftSize.Prop },
            };

            var dataHelper = SetupData(DefaultAircraftTypes, spaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new BookingRequest { ParkingSpaceId = 1, AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            Assert.Throws<Exception>(() => processor.BookAppointment(request));
        }
        
        [Test]
        public void BookAppointment_ThrowsErrorWhenSpaceNotFree()
        {
            var aircraftTypeId = "A380";
            var spaces = new List<ParkingSpace>
            {
                new ParkingSpace { Id = 1, Size = AircraftSize.Jumbo, Appointments = new List<BookingAppointment> { DefaultAppointment } },
            };

            var dataHelper = SetupData(DefaultAircraftTypes, spaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new BookingRequest { ParkingSpaceId = 1, AircraftTypeId = aircraftTypeId, Days = 1, Hours = 1 };
            Assert.Throws<Exception>(() => processor.BookAppointment(request));
        }
        
        [Test]
        public void BookAppointment_ThrowsErrorWhenNoDuration()
        {
            var aircraftTypeId = "A380";
            var dataHelper = SetupData(DefaultAircraftTypes, DefaultParkingSpaces);
            var processor = new AppointmentProcessor(dataHelper.Object, MockDateTime());
            var request = new BookingRequest { ParkingSpaceId = 76, AircraftTypeId = aircraftTypeId, Days = 0, Hours = 0 };
            Assert.Throws<Exception>(() => processor.BookAppointment(request));
        }
    }
}