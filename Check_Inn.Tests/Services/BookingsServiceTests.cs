using Check_Inn.DAL;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.Tests.Helpers;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Check_Inn.Tests.Services
{
    [TestFixture]
    public class BookingsServiceTests
    {
        private Mock<ICheckInnContext> _mockContext;
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

            _mockContext = new Mock<ICheckInnContext>();
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

        // [Test]
        // public void SearchBooking_WithValidSearchTerm_ShouldReturnFilteredResults()
        // {
        //     // Act - Search for "John" which should match "John Doe"
        //     var result = _service.SearchBooking("John", null, 1, 10);
        //
        //     // Assert
        //     result.Should().HaveCount(1);
        //     result.First().GuestName.Should().Be("John Doe");
        // }

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
        public void SaveBooking_ShouldAddToContextAndReturnTrue()
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

            // Setup for the email service - make EmailService methods virtual or create an interface
            // For now, we'll just verify the booking is saved without testing email functionality
            
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
            var mockEntry = new Mock<IDbEntityEntry<Booking>>();
            _mockContext.Setup(c => c.Entry(bookingToUpdate)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.UpdateBooking(bookingToUpdate);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Modified, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void DeleteBooking_ShouldSetStateToDeletedAndReturnTrue()
        {
            // Arrange
            var bookingToDelete = _testBookings.First();
            var mockEntry = new Mock<IDbEntityEntry<Booking>>();
            _mockContext.Setup(c => c.Entry(bookingToDelete)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.DeleteBooking(bookingToDelete);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Deleted, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void IsAccomodationAvailable_WithNonOverlappingDates_ShouldReturnTrue()
        {
            // Arrange - check availability for dates that don't overlap with existing bookings
            var checkFromDate = DateTime.Today.AddDays(20); // Far from existing bookings
            var duration = 2;

            // Act
            var result = _service.IsAccomodationAvailable(1, checkFromDate, duration);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsAccomodationAvailable_WithOverlappingDates_ShouldReturnFalse()
        {
            // Arrange - check availability for dates that overlap with existing bookings
            // Booking ID 1 is from Today+1 for 3 days, so checking Today+2 should overlap
            var checkFromDate = DateTime.Today.AddDays(2);
            var duration = 2;

            // Act
            var result = _service.IsAccomodationAvailable(1, checkFromDate, duration);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasPendingPayment_WithPendingPayment_ShouldReturnTrue()
        {
            // Act
            var result = _service.HasPendingPayment(2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void HasPendingPayment_WithoutPendingPayment_ShouldReturnFalse()
        {
            // Act
            var result = _service.HasPendingPayment(1);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HasCompletedPayment_WithCompletedPayment_ShouldReturnTrue()
        {
            // Act
            var result = _service.HasCompletedPayment(1);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void HasCompletedPayment_WithoutCompletedPayment_ShouldReturnFalse()
        {
            // Act
            var result = _service.HasCompletedPayment(2);

            // Assert
            result.Should().BeFalse();
        }
    }
}
