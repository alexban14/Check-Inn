using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.ViewModels;
using Stripe;

namespace Check_Inn.Controllers
{
    public class PaymentController : Controller
    {
        private readonly BookingsService _bookingsService;
        private readonly AccomodationsService _accomodationsService;
        private readonly AccomodationPackagesService _accomodationPackagesService;
        private readonly PaymentService _paymentService;
        private readonly StripeService _stripeService;
        private readonly EmailService _emailService;

        public PaymentController()
        {
            _bookingsService = new BookingsService();
            _accomodationsService = new AccomodationsService();
            _accomodationPackagesService = new AccomodationPackagesService();
            _paymentService = new PaymentService();
            _stripeService = new StripeService();
            _emailService = new EmailService();
        }

        [HttpGet]
        public async Task<ActionResult> ProcessPayment(int bookingId)
        {
            var booking = _bookingsService.GetBookingByID(bookingId);
            if (booking == null)
            {
                return HttpNotFound();
            }

            var accomodation = _accomodationsService.GetAccomodationByID(booking.AccomodationID);
            var package = _accomodationPackagesService.GetAccomodationPackageByID(accomodation.AccomodationPackageID);

            var paymentIntent = await _stripeService.CreatePaymentIntentAsync(booking, package);

            var model = new PaymentViewModel
            {
                BookingID = booking.ID,
                BookingReference = $"BOOK-{booking.ID}",
                AccomodationName = accomodation.Name,
                AccomodationPackageName = package.Name,
                AccomodationImage = accomodation.Image,
                CheckInDate = booking.FromDate,
                CheckOutDate = booking.FromDate.AddDays(booking.Duration),
                Duration = booking.Duration,
                FeePerNight = package.FeePerNight,
                TotalAmount = package.FeePerNight * booking.Duration,
                GuestName = booking.GuestName,
                Email = booking.Email,
                NoOfAdults = booking.NoOfAdults,
                NoOfChildren = booking.NoOfChildren,
                ClientSecret = paymentIntent.ClientSecret,
                PublicKey = System.Configuration.ConfigurationManager.AppSettings["StripePublicKey"]
            };

            // Create a pending payment record
            var payment = new Payment
            {
                BookingID = booking.ID,
                Amount = model.TotalAmount,
                PaymentStatus = "Pending",
                StripePaymentIntentId = paymentIntent.Id,
                Notes = "Payment initiated"
            };
            
            _paymentService.SavePayment(payment);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmPayment(int bookingId, string paymentIntentId)
        {
            var booking = _bookingsService.GetBookingByID(bookingId);
            if (booking == null)
            {
                return Json(new { success = false, message = "Booking not found" });
            }

            try
            {
                var paymentIntent = await _stripeService.ConfirmPaymentIntentAsync(paymentIntentId);
                
                if (paymentIntent.Status == "succeeded")
                {
                    // Update the payment record
                    var payment = _paymentService.GetPaymentsByBookingID(bookingId)
                        .FirstOrDefault(p => p.StripePaymentIntentId == paymentIntentId);
                    
                    if (payment != null)
                    {
                        payment.PaymentStatus = "Completed";
                        payment.StripeChargeId = paymentIntent.Id;
                        payment.Notes = "Payment completed successfully";
                        _paymentService.UpdatePayment(payment);
                    }

                    // Send confirmation email
                    var accomodation = _accomodationsService.GetAccomodationByID(booking.AccomodationID);
                    var package = _accomodationPackagesService.GetAccomodationPackageByID(accomodation.AccomodationPackageID);
                    await _emailService.SendBookingConfirmationAsync(booking, accomodation, package);

                    return Json(new { success = true, message = "Payment successful", redirectUrl = Url.Action("PaymentSuccess", new { bookingId = bookingId }) });
                }
                else
                {
                    // Update the payment record with failure status
                    var payment = _paymentService.GetPaymentsByBookingID(bookingId)
                        .FirstOrDefault(p => p.StripePaymentIntentId == paymentIntentId);
                    
                    if (payment != null)
                    {
                        payment.PaymentStatus = "Failed";
                        payment.Notes = $"Payment failed with status: {paymentIntent.Status}";
                        _paymentService.UpdatePayment(payment);
                    }

                    return Json(new { success = false, message = "Payment was not successful. Please try again." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public ActionResult PaymentSuccess(int bookingId)
        {
            var booking = _bookingsService.GetBookingByID(bookingId);
            if (booking == null)
            {
                return HttpNotFound();
            }

            var accomodation = _accomodationsService.GetAccomodationByID(booking.AccomodationID);
            var package = _accomodationPackagesService.GetAccomodationPackageByID(accomodation.AccomodationPackageID);

            var model = new PaymentViewModel
            {
                BookingID = booking.ID,
                BookingReference = $"BOOK-{booking.ID}",
                AccomodationName = accomodation.Name,
                AccomodationPackageName = package.Name,
                CheckInDate = booking.FromDate,
                CheckOutDate = booking.FromDate.AddDays(booking.Duration),
                Duration = booking.Duration,
                TotalAmount = package.FeePerNight * booking.Duration,
                GuestName = booking.GuestName,
                Email = booking.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> WebhookHandler()
        {
            var json = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            
            try
            {
                var stripeEvent = _stripeService.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"]
                );

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Handle successful payment
                    if (paymentIntent != null && paymentIntent.Metadata.TryGetValue("BookingId", out var bookingIdStr) && int.TryParse(bookingIdStr, out var bookingId))
                    {
                        var payment = _paymentService.GetPaymentsByBookingID(bookingId)
                            .FirstOrDefault(p => p.StripePaymentIntentId == paymentIntent.Id);
                        
                        if (payment != null)
                        {
                            payment.PaymentStatus = "Completed";
                            payment.StripeChargeId = paymentIntent.Id;
                            payment.Notes = "Payment completed via webhook";
                            _paymentService.UpdatePayment(payment);
                        }
                    }
                }
                else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Handle failed payment
                    if (paymentIntent != null && paymentIntent.Metadata.TryGetValue("BookingId", out var bookingIdStr) && int.TryParse(bookingIdStr, out var bookingId))
                    {
                        var payment = _paymentService.GetPaymentsByBookingID(bookingId)
                            .FirstOrDefault(p => p.StripePaymentIntentId == paymentIntent.Id);
                        
                        if (payment != null)
                        {
                            payment.PaymentStatus = "Failed";
                            payment.Notes = "Payment failed via webhook";
                            _paymentService.UpdatePayment(payment);
                        }
                    }
                }

                return new HttpStatusCodeResult(200);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400);
            }
        }
    }
}
