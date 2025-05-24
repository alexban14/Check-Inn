using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Check_Inn.Entities;
using Check_Inn.Services;
using Check_Inn.ViewModels;
using Stripe;
using Stripe.Checkout;

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

        public PaymentController(
            BookingsService bookingsService,
            AccomodationsService accomodationsService,
            AccomodationPackagesService accomodationPackagesService,
            PaymentService paymentService,
            StripeService stripeService,
            EmailService emailService
        )
        {
            _bookingsService = bookingsService;
            _accomodationsService = accomodationsService;
            _accomodationPackagesService = accomodationPackagesService;
            _paymentService = paymentService;
            _stripeService = stripeService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult> ProcessPaymentOwn(int bookingId)
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
                PublicKey = System.Configuration.ConfigurationManager.AppSettings["StripePublishableKey"]
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

        [HttpGet]
        public async Task<ActionResult> ProcessPayment(int bookingId)
        {
            var booking = _bookingsService.GetBookingByID(bookingId);
            var accomodation = _accomodationsService.GetAccomodationByID(booking.AccomodationID);
            var package = _accomodationPackagesService.GetAccomodationPackageByID(accomodation.AccomodationPackageID);

            Session session = await _stripeService.CreateCheckoutSessionAsync(booking, package);

            var payments = _paymentService.GetPaymentsByBookingID(bookingId);

            if (payments.Count > 0)
            {
                var payment = payments.First();

            } else {
                var payment = new Payment
                {
                    BookingID = booking.ID,
                    Amount = package.FeePerNight * booking.Duration,
                    PaymentStatus = "Pending",
                    StripePaymentIntentId = session.PaymentIntentId,
                    Notes = "Payment initiated"
                };

                _paymentService.SavePayment(payment);

            }


            return Redirect(session.Url);
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
            Request.InputStream.Position = 0;
            var json = new System.IO.StreamReader(Request.InputStream).ReadToEnd();
            var stripeSignHeader = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = _stripeService.ConstructEvent(json, stripeSignHeader);


                System.Diagnostics.Debug.WriteLine($"Handling event type: {stripeEvent.Type}");
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        await _stripeService.HandlePaymentIntentSucceeded(stripeEvent);
                        break;

                    case Events.PaymentIntentPaymentFailed:
                        await _stripeService.HandlePaymentIntentFailed(stripeEvent);
                        break;

                    case Events.CheckoutSessionCompleted:
                        await _stripeService.HandleCheckoutSessionCompleted(stripeEvent);
                        break;

                    case Events.ChargeSucceeded:
                        await _stripeService.HandleChargeSucceeded(stripeEvent);
                        break;

                    case Events.ChargeUpdated:
                        await _stripeService.HandleChargeUpdated(stripeEvent);
                        break;

                    case Events.PaymentIntentCreated:
                        System.Diagnostics.Debug.WriteLine($"PaymentIntent created: {stripeEvent.Id}");
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                return new HttpStatusCodeResult(200);
            }
            catch (StripeException e)
            {
                System.Diagnostics.Debug.WriteLine($"Stripe error: {e.StripeError?.Message ?? e.Message}");
                return new HttpStatusCodeResult(400);
            }
            /*
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Webhook error: {e.Message}");
                return new HttpStatusCodeResult(500);
            }
            */
        }
    }
}
