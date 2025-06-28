using Check_Inn.DAL;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.Tests.Helpers;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace Check_Inn.Tests.Services
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private Mock<ICheckInnContext> _mockContext;
        private Mock<DbSet<Payment>> _mockPaymentsSet;
        private PaymentService _service;
        private List<Payment> _testData;

        [SetUp]
        public void SetUp()
        {
            _testData = new List<Payment>
            {
                new Payment { ID = 1, BookingID = 1, Amount = 100, PaymentStatus = "Completed", StripePaymentIntentId = "pi_1" },
                new Payment { ID = 2, BookingID = 2, Amount = 200, PaymentStatus = "Pending", StripePaymentIntentId = "pi_2" },
                new Payment { ID = 3, BookingID = 1, Amount = 50, PaymentStatus = "Completed", StripePaymentIntentId = "pi_3" }
            };

            _mockPaymentsSet = MockDbSetHelper.CreateMockDbSet(_testData);
            _mockContext = new Mock<ICheckInnContext>();
            _mockContext.Setup(c => c.Payments).Returns(_mockPaymentsSet.Object);
            _service = new PaymentService(_mockContext.Object);
        }

        [Test]
        public void GetPaymentByID_WithValidID_ShouldReturnCorrectPayment()
        {
            // Act
            var result = _service.GetPaymentByID(1);

            // Assert
            result.Should().NotBeNull();
            result.ID.Should().Be(1);
        }

        [Test]
        public void GetPaymentsByBookingID_WithValidBookingID_ShouldReturnPayments()
        {
            // Act
            var result = _service.GetPaymentsByBookingID(1);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.BookingID == 1).Should().BeTrue();
        }

        [Test]
        public void GetPaymentByStripePaymentIntentId_WithValidIntentId_ShouldReturnPayment()
        {
            // Act
            var result = _service.GetPaymentByStripePaymentIntentId("pi_1");

            // Assert
            result.Should().NotBeNull();
            result.StripePaymentIntentId.Should().Be("pi_1");
        }

        [Test]
        public void SavePayment_ShouldAddToContextAndReturnTrue()
        {
            // Arrange
            var newPayment = new Payment { ID = 4, BookingID = 3, Amount = 300, PaymentStatus = "Pending" };
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.SavePayment(newPayment);

            // Assert
            result.Should().BeTrue();
            _mockPaymentsSet.Verify(m => m.Add(newPayment), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdatePayment_ShouldSetStateToModifiedAndReturnTrue()
        {
            // Arrange
            var paymentToUpdate = _testData.First();
            var mockEntry = new Mock<IDbEntityEntry<Payment>>();
            _mockContext.Setup(c => c.Entry(paymentToUpdate)).Returns(mockEntry.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.UpdatePayment(paymentToUpdate);

            // Assert
            result.Should().BeTrue();
            mockEntry.VerifySet(e => e.State = EntityState.Modified, Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Test]
        public void DeletePayment_WithValidID_ShouldRemoveFromContextAndReturnTrue()
        {
            // Arrange
            var paymentToDelete = _testData.First();
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            var result = _service.DeletePayment(paymentToDelete.ID);

            // Assert
            result.Should().BeTrue();
            _mockPaymentsSet.Verify(m => m.Remove(paymentToDelete), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
}
