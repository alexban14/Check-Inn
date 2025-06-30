using Check_Inn.DAL;
using Check_Inn.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Check_Inn.Tests.Helpers
{
    public static class TestDataHelper
    {
        public static List<AccomodationType> GetTestAccomodationTypes()
        {
            return new List<AccomodationType>
            {
                new AccomodationType 
                { 
                    ID = 1, 
                    Name = "Hotel", 
                    Description = "Standard hotel room with all amenities", 
                    Image = "hotel.jpg" 
                },
                new AccomodationType 
                { 
                    ID = 2, 
                    Name = "Apartment", 
                    Description = "Furnished apartment rental for extended stays", 
                    Image = "apartment.jpg" 
                },
                new AccomodationType 
                { 
                    ID = 3, 
                    Name = "Villa", 
                    Description = "Luxury villa with private pool and garden", 
                    Image = "villa.jpg" 
                }
            };
        }

        public static List<AccomodationPackage> GetTestAccomodationPackages()
        {
            return new List<AccomodationPackage>
            {
                new AccomodationPackage 
                { 
                    ID = 1, 
                    Name = "Standard Package", 
                    AccomodationTypeID = 1, 
                    NoOfRoom = 1,
                    FeePerNight = 100.00m 
                },
                new AccomodationPackage 
                { 
                    ID = 2,
                    Name = "Premium Package", 
                    AccomodationTypeID = 1, 
                    NoOfRoom = 2,
                    FeePerNight = 200.00m 
                },
                new AccomodationPackage 
                { 
                    ID = 3, 
                    Name = "Villa Package", 
                    AccomodationTypeID = 3, 
                    NoOfRoom = 4,
                    FeePerNight = 500.00m 
                }
            };
        }

        public static List<Accomodation> GetTestAccomodations()
        {
            return new List<Accomodation>
            {
                new Accomodation 
                { 
                    ID = 1, 
                    Name = "Room 101", 
                    AccomodationPackageID = 1,
                    Description = "Comfortable single room with city view",
                    Image = "room101.jpg"
                },
                new Accomodation 
                { 
                    ID = 2, 
                    Name = "Room 102", 
                    AccomodationPackageID = 1,
                    Description = "Cozy single room with garden view",
                    Image = "room102.jpg"
                },
                new Accomodation 
                { 
                    ID = 3, 
                    Name = "Premium Suite", 
                    AccomodationPackageID = 2,
                    Description = "Luxurious suite with balcony and sea view",
                    Image = "premium_suite.jpg"
                }
            };
        }

        public static List<Booking> GetTestBookings()
        {
            return new List<Booking>
            {
                new Booking 
                { 
                    ID = 1, 
                    AccomodationID = 1, 
                    GuestName = "John Doe", 
                    Email = "john@example.com",
                    FromDate = DateTime.Today.AddDays(1),
                    Duration = 3,
                    NoOfAdults = 2,
                    NoOfChildren = 1,
                    AdditionalInfo = "Late check-in requested"
                },
                new Booking 
                { 
                    ID = 2, 
                    AccomodationID = 1, 
                    GuestName = "Jane Smith", 
                    Email = "jane@example.com",
                    FromDate = DateTime.Today.AddDays(10),
                    Duration = 2,
                    NoOfAdults = 1,
                    NoOfChildren = 0,
                    AdditionalInfo = "Business trip"
                },
                new Booking 
                { 
                    ID = 3, 
                    AccomodationID = 2, 
                    GuestName = "Bob Johnson", 
                    Email = "john@example.com",
                    FromDate = DateTime.Today.AddDays(5),
                    Duration = 1,
                    NoOfAdults = 2,
                    NoOfChildren = 2,
                    AdditionalInfo = "Family vacation"
                }
            };
        }
    }

    public static class MockDbSetHelper
    {
        public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            // Setup Add method
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(data.Add);

            // Setup Find method
            mockSet.Setup(m => m.Find(It.IsAny<object[]>()))
                   .Returns<object[]>(ids => data.FirstOrDefault(d => GetEntityId(d).Equals(ids[0])));

            return mockSet;
        }

        private static object GetEntityId<T>(T entity)
        {
            var idProperty = typeof(T).GetProperty("ID");
            return idProperty?.GetValue(entity);
        }
    }
}
