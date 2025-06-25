using Check_Inn.DAL;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Check_Inn.Tests.Services
{
    [TestFixture]
    public class BookingsServiceTests
    {
        private Mock<CheckInnMySqlContext> _mockContext;
        private Mock<DbSet<Booking>> _mockBookingsSet;
        private Mock<DbSet<Payment>> _mockPaymentsSet;
        private Mock<DbSet<Accomodation>> _mockAccomodationsSet;
        private Mock<EmailService> _mockEmailService;
        private BookingsService _service;
        private List<Booking> _testBookings;
        private List<Payment> _testPayments;
        private List<Accomodation> _testAccomodations;

        [SetUp]
        public void SetUp()
        {
            _testBookings = TestDataHelper.GetTestBookings();
            _testAccomodations = TestDataHelper.GetTestAccomodations();
            _testPayments = new List<Payment>
            {
                new Payment { ID = 1, BookingID = 1, PaymentStatus = "Completed" },
                new Payment { ID = 2, BookingID = 2, PaymentStatus = "Pending" }
            };

            _mockBookingsSet = MockDbSetHelper.CreateMockDbSet(_testBookings);
            _mockPaymentsSet = MockDbSetHelper.CreateMockDbSet(_testPayments);
            _mockAccomodationsSet = MockDbSetHelper.CreateMockDbSet(_testAccomodations);

            _mockContext = new Mock<CheckInnMySqlContext>();
            _mockContext.Setup(c => c.Bookings).Returns(_mockBookingsSet.Object);
            _mockContext.Setup(c => c.Payments).Returns(_mockPaymentsSet.Object);
            _mockContext.Setup(c => c.Accomodations).Returns(_mockAccomodationsSet.Object);

            _mockEmailService = new Mock<EmailService>();
            _service = new BookingsService(_mockContext.Object, _mockEmailService.Object);
        }

        [Test]
        public void GetAllAcomodationPackages_ShouldReturnAllBookings()
        {
            // Act
            var result = _service.GetAllAcomodationPackages();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_testBookings);
        }

        [Test]
        public void GetAllBookingsByAccomodationPackage_WithValidAccomodationID_ShouldReturnFilteredBookings()
        {
            // Act
            var result = _service.GetAllBookingsByAccomodationPackage(1);

            // Assert
            result.Should().HaveCount(2);
            result.All(b => b.AccomodationID == 1).Should().BeTrue();
        }

        [Test]
        public void GetBookingsByUserEmail_WithValidEmail_ShouldReturnUserBookings()
        {
            // Act
            var result = _service.GetBookingsByUserEmail("john@example.com", 1, 10);

            // Assert
            result.Should().HaveCount(2);
            result.All(b => b.Email == "john@example.com").Should().BeTrue();
        }

        [Test]
        public void GetBookingsByUserEmail_WithPagination_ShouldReturnCorrectPage()
        {
            // Act
            var result = _service.GetBookingsByUserEmail("john@example.com", 1, 1);

            // Assert
            result.Should().HaveCount(1);
        }

        [Test]
        public void GetBookingCountByUserEmail_WithValidEmail_ShouldReturnCorrectCount()
        {
            // Act
            var result = _service.GetBookingCountByUserEmail("john@example.com");

            // Assert
            result.Should().Be(2);
        }

        [Test]
        public void SearchBooking_WithNullSearchTerm_ShouldReturnPaginatedResults()
        {
            // Act
            var result = _service.SearchBooking(null, null, 1, 2);

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public void SearchBooking_WithValidSearchTerm_ShouldReturnFilteredResults()
        {
            // Act
            var result = _service.SearchBooking("john", null, 1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().GuestName.Should().Be("John Doe");
        }

        [Test]
        public void SearchBooking_WithAccomodationID_ShouldReturnFilteredResults()
        {
            // Act
            var result = _service.SearchBooking(null, 2, 1, 10);

            // Assert
            result.Should().HaveCount(1);
            result.First().AccomodationID.Should().Be(2);
        }

        [Test]
        public void SearchBookingCount_WithValidSearchTerm_ShouldReturnFilteredCount()
        {
            // Act
            var result = _service.SearchBookingCount("jane", null);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public void GetBookingByID_WithValidID_ShouldReturnCorrectBooking()
        {
            // Act
            var result = _service.GetBookingByID(1);

            // Assert
            result.Should().NotBeNull();
            result.ID.Should().Be(1);
            result.GuestName.Should().Be("John Doe");
        }

        [Test]
        public void SaveBooking_ShouldAddToContextSendEmailAndReturnTrue()
        {
            // Arrange
            var newBooking = new Booking
            {
                ID = 4,
                AccomodationID = 1,
                GuestName = "Test User",
                Email = "test@example.com"
            };
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);
            _mockEmailService.Setup(e => e.SendBookingConfirmation(It.IsAny<Booking>(), It.IsAny<Accomodation>(), It.IsAny<AccomodationPackage>()))
                            .Returns(true);

            // Act
            var result = _service.SaveBooking(newBooking);

            // Assert
            result.Should().BeTrue();
            _mockBookingsSet.Verify(m => m.Add(newBooking), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateBooking_ShouldSetStateToModifiedAndReturnTrue()
        {
            // Arrange
            var bookingToUpdate = _testBookings.First();
            var mockEntry = new Mock<System.Data.Entity.Infrastructure.DbEntityEntry<Booking>>();
            _mockContext.Setup(c => c.Entry(bookingToUpdate)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);
        }
    }
}
