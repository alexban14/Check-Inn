using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Stripe;
using Check_Inn.Entities;

namespace Check_Inn.Services
{
    public class StripeService
    {
        private readonly string _apiKey;
        private readonly string _webhookSecret;

        public StripeService()
        {
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
            return EventUtility.ConstructEvent(json, stripeSignature, _webhookSecret);
        }
    }
}
