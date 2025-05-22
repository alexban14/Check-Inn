using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Stripe;
using Check_Inn.Entities;
using Stripe.Checkout;
using System.Linq;

namespace Check_Inn.Services
{
    public class StripeService
    {
        private readonly PaymentService _paymentService;
        private readonly string _callbackUrl;
        private readonly string _apiKey;
        private readonly string _webhookSecret;

        public StripeService()
        {
            _paymentService = new PaymentService();

            _callbackUrl = System.Configuration.ConfigurationManager.AppSettings["CallbackUrl"];
            _apiKey = System.Configuration.ConfigurationManager.AppSettings["StripeSecretKey"];
            _webhookSecret = System.Configuration.ConfigurationManager.AppSettings["StripeWebhookSecret"];
            
            StripeConfiguration.ApiKey = _apiKey;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(Booking booking, AccomodationPackage package)
        {
            var amount = (long)(package.FeePerNight * booking.Duration * 100); // Convert to cents

            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                {
                    { "BookingId", booking.ID.ToString() },
                    { "AccomodationName", booking.Accomodation.Name },
                    { "GuestName", booking.GuestName },
                    { "CheckInDate", booking.FromDate.ToString("yyyy-MM-dd") },
                    { "Duration", booking.Duration.ToString() }
                },
                Description = $"Booking #{booking.ID} - {booking.Accomodation.Name} for {booking.Duration} night(s)"
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public async Task<Session> CreateCheckoutSessionAsync(Booking booking, AccomodationPackage package)
        {
            var amount = (long)(package.FeePerNight * booking.Duration * 100); // Convert to cents

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = amount,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{booking.Accomodation.Name} (Booking #{booking.ID})",
                                Description = $"{booking.Duration} night(s) - Check-in: {booking.FromDate:yyyy-MM-dd}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"{_callbackUrl}/Payment/PaymentSuccess?bookingId={booking.ID}",
                CancelUrl = $"{_callbackUrl}/Accomodations/BookAccomodation?accomodationPackageID={package.ID}&accomodationID={booking.AccomodationID}",
                Metadata = new Dictionary<string, string>
                {
                    { "BookingId", booking.ID.ToString() },
                    { "AccomodationName", booking.Accomodation.Name },
                    { "GuestName", booking.GuestName }
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "BookingId", booking.ID.ToString() },
                    }
                }
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }


        public async Task<PaymentIntent> ConfirmPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return await service.GetAsync(paymentIntentId);
        }

        public async Task<Refund> CreateRefundAsync(string paymentIntentId)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var service = new RefundService();
            return await service.CreateAsync(options);
        }

        public Event ConstructEvent(string json, string stripeSignature)
        {
            return EventUtility.ConstructEvent(
                json, 
                stripeSignature, 
                _webhookSecret,
                throwOnApiVersionMismatch: false
            );
        }

        public async Task HandlePaymentIntentSucceeded(Event stripeEvent)
        {
            System.Diagnostics.Debug.WriteLine("start: HandlePaymentIntentSucceeded");
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;


            if (
                !paymentIntent.Metadata.TryGetValue("BookingId", out var bookingIdStr) ||
                !int.TryParse(bookingIdStr, out var bookingId)
            ) {
                var payment = _paymentService.GetPaymentByStripePaymentIntentId(paymentIntent.Id);

                if (payment != null)
                {
                    payment.PaymentStatus = "Completed";
                    payment.StripeChargeId = paymentIntent.Id;
                    payment.Notes = "Payment completed via webhook (no metadata)";
                    _paymentService.UpdatePayment(payment);
                }
                return;
            }

            var paymentWithMetadata = _paymentService.GetPaymentsByBookingID(bookingId)
                .FirstOrDefault(p => p.StripePaymentIntentId == paymentIntent.Id);

            if (paymentWithMetadata != null)
            {
                paymentWithMetadata.PaymentStatus = "Completed";
                paymentWithMetadata.StripeChargeId = paymentIntent.Id;
                paymentWithMetadata.Notes = "Payment completed via webhook";
                _paymentService.UpdatePayment(paymentWithMetadata);
            }

            System.Diagnostics.Debug.WriteLine("end: HandlePaymentIntentSucceeded");
        }

        public async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            System.Diagnostics.Debug.WriteLine("start: HandleCheckoutSessionCompleted");
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;

            if (session.Metadata.TryGetValue("BookingId", out var bookingIdStr) &&
                int.TryParse(bookingIdStr, out var bookingId))
            {
                var payment = _paymentService.GetPaymentsByBookingID(bookingId)
                    .FirstOrDefault(p => p.StripePaymentIntentId == session.PaymentIntentId);

                if (payment != null)
                {
                    payment.PaymentStatus = "Completed";
                    payment.StripeChargeId = session.PaymentIntentId;
                    payment.Notes = "Payment completed via Checkout webhook";
                    _paymentService.UpdatePayment(payment);
                }
            }
            System.Diagnostics.Debug.WriteLine("end: HandleCheckoutSessionCompleted");
        }

        public async Task HandlePaymentIntentFailed(Event stripeEvent)
        {
            System.Diagnostics.Debug.WriteLine("start: HandlePaymentIntentFailed");
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            if (paymentIntent.Metadata.TryGetValue("BookingId", out var bookingIdStr) &&
                int.TryParse(bookingIdStr, out var bookingId))
            {
                var payment = _paymentService.GetPaymentsByBookingID(bookingId)
                    .FirstOrDefault(p => p.StripePaymentIntentId == paymentIntent.Id);

                if (payment != null)
                {
                    payment.PaymentStatus = "Failed";
                    payment.Notes = $"Payment failed: {paymentIntent.LastPaymentError?.Message ?? "Unknown reason"}";
                    _paymentService.UpdatePayment(payment);
                }
            }
            System.Diagnostics.Debug.WriteLine("end: HandlePaymentIntentFailed");
        }

        public async Task HandleChargeSucceeded(Event stripeEvent)
        {
            System.Diagnostics.Debug.WriteLine("start: HandleChargeSucceeded");
            var charge = stripeEvent.Data.Object as Charge;
            if (charge == null) return;

            if (string.IsNullOrEmpty(charge.PaymentIntentId))
            {
                System.Diagnostics.Debug.WriteLine($"Charge succeeded without PaymentIntent: {charge.Id}");
                return;
            }

            var payment = _paymentService.GetPaymentByStripePaymentIntentId(charge.PaymentIntentId);
            if (payment != null)
            {
                payment.StripeChargeId = charge.Id;
                payment.Notes = $"Charge succeeded: {charge.Id}";
                _paymentService.UpdatePayment(payment);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No payment found for PaymentIntent: {charge.PaymentIntentId}");
            }
            System.Diagnostics.Debug.WriteLine("start: HandleChargeSucceeded");
        }

        public async Task HandleChargeUpdated(Event stripeEvent)
        {
            System.Diagnostics.Debug.WriteLine("start: HandleChargeUpdated");
            var charge = stripeEvent.Data.Object as Charge;

            if (charge == null)
            {
                System.Diagnostics.Debug.WriteLine("Error: Charge object is null");
                return;
            }

            // Log charge details safely
            System.Diagnostics.Debug.WriteLine($"Charge ID: {charge.Id}");
            System.Diagnostics.Debug.WriteLine($"Status: {charge.Status}");
            System.Diagnostics.Debug.WriteLine($"PaymentIntent: {charge.PaymentIntentId}");
            System.Diagnostics.Debug.WriteLine($"Amount: {charge.Amount}");

            // Find the related payment
            var payment = _paymentService.GetPaymentByStripeChargeId(charge.Id) ??
                         _paymentService.GetPaymentByStripePaymentIntentId(charge.PaymentIntentId);

            if (payment == null)
            {
                System.Diagnostics.Debug.WriteLine($"No payment found for charge: {charge.Id}");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Found payment ID: {payment.ID}");
            System.Diagnostics.Debug.WriteLine($"Current status: {payment.PaymentStatus}");

            if (payment != null)
            {
                payment.Notes = $"Charge updated: {charge.Status}";

                switch (charge.Status)
                {
                    case "succeeded":
                        payment.PaymentStatus = "Completed";
                        break;
                    case "failed":
                        payment.PaymentStatus = "Failed";
                        break;
                    case "pending":
                        payment.PaymentStatus = "Pending";
                        break;
                }

                System.Diagnostics.Debug.Write(payment);

                _paymentService.UpdatePayment(payment);

                System.Diagnostics.Debug.WriteLine($"Updated payment {payment.ID} for charge {charge.Id}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No payment found for updated charge: {charge.Id}");
            }
            System.Diagnostics.Debug.WriteLine("start: HandleChargeUpdated");
        }
    }
}
