using NUnit.Framework;
using Moq;
using Check_Inn.Controllers;
using Check_Inn.Services;
using Check_Inn.Entities;
using System.Web.Mvc;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;

namespace Check_Inn.Tests.Controllers
{
    [TestFixture]
    public class PaymentControllerTests
    {
        private PaymentController _controller;
        private Mock<BookingsService> _mockBookingsService;
        private Mock<AccomodationsService> _mockAccomodationsService;
        private Mock<AccomodationPackagesService> _mockPackagesService;
        private Mock<PaymentService> _mockPaymentService;
        private Mock<StripeService> _mockStripeService;
        private Mock<EmailService> _mockEmailService;

        [SetUp]
        public void Setup()
        {
            _mockBookingsService = new Mock<BookingsService>();
            _mockAccomodationsService = new Mock<AccomodationsService>();
            _mockPackagesService = new Mock<AccomodationPackagesService>();
            _mockPaymentService = new Mock<PaymentService>();
            _mockStripeService = new Mock<StripeService>();
            _mockEmailService = new Mock<EmailService>();


            _controller = new PaymentController(
                _mockBookingsService.Object,
                _mockAccomodationsService.Object,
                _mockPackagesService.Object,
                _mockPaymentService.Object,
                _mockStripeService.Object,
                _mockEmailService.Object);
        }

        [Test]
        public async Task ProcessPayment_NewBooking_RedirectsToStripe()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking { ID = bookingId, AccomodationID = 1 };
            var accomodation = new Accomodation { AccomodationPackageID = 1 };
            var package = new AccomodationPackage { FeePerNight = 100 };
            var session = new Session { Url = "https://stripe.com/checkout" };

            _mockBookingsService.Setup(x => x.GetBookingByID(bookingId)).Returns(booking);
            _mockAccomodationsService.Setup(x => x.GetAccomodationByID(booking.AccomodationID)).Returns(accomodation);
            _mockPackagesService.Setup(x => x.GetAccomodationPackageByID(accomodation.AccomodationPackageID)).Returns(package);
            _mockStripeService.Setup(x => x.CreateCheckoutSessionAsync(booking, package)).ReturnsAsync(session);
            _mockPaymentService.Setup(x => x.GetPaymentsByBookingID(bookingId)).Returns(new List<Payment>());

            // Act
            var result = await _controller.ProcessPayment(bookingId);

            // Assert
            Assert.IsInstanceOf<RedirectResult>(result);
            var redirect = (RedirectResult)result;
            Assert.AreEqual(session.Url, redirect.Url);
            _mockPaymentService.Verify(x => x.SavePayment(It.IsAny<Payment>()), Times.Once);
        }

        [Test]
        public async Task ConfirmPayment_SuccessfulPayment_UpdatesStatus()
        {
            // Arrange
            var bookingId = 1;
            var paymentIntentId = "pi_test";
            var booking = new Booking { ID = bookingId, AccomodationID = 1 };
            var payment = new Payment { StripePaymentIntentId = paymentIntentId };
            var paymentIntent = new PaymentIntent { Status = "succeeded", Id = paymentIntentId };

            _mockBookingsService.Setup(x => x.GetBookingByID(bookingId)).Returns(booking);
            _mockStripeService.Setup(x => x.ConfirmPaymentIntentAsync(paymentIntentId)).ReturnsAsync(paymentIntent);
            _mockPaymentService.Setup(x => x.GetPaymentsByBookingID(bookingId))
                .Returns(new List<Payment> { payment });

            // Act
            var result = await _controller.ConfirmPayment(bookingId, paymentIntentId);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            _mockPaymentService.Verify(x => x.UpdatePayment(It.Is<Payment>(p => 
                p.PaymentStatus == "Completed")), Times.Once);
            _mockEmailService.Verify(x => x.SendBookingConfirmationAsync(
                It.IsAny<Booking>(), It.IsAny<Accomodation>(), It.IsAny<AccomodationPackage>()), Times.Once);
        }
    }
}
