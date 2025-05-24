using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Check_Inn.DAL;
using Check_Inn.Entities;

namespace Check_Inn.Services
{
    public class PaymentService
    {
        private CheckInnMySqlContext _context;

        public PaymentService(CheckInnMySqlContext dbContext)
        {
            _context = dbContext;
        }

        public Payment GetPaymentByID(int id)
        {
            return _context.Payments.Find(id);
        }

        public List<Payment> GetPaymentsByBookingID(int bookingID)
        {
            return _context.Payments.Where(p => p.BookingID == bookingID).ToList();
        }

        public Payment GetPaymentByStripePaymentIntentId(string stripePaymentIntentId)
        {
            return _context.Payments.Where(p => p.StripePaymentIntentId == stripePaymentIntentId).First();
        }

        public Payment GetPaymentByStripeChargeId(string chargeId)
        {
            return _context.Payments.Where(p => p.StripeChargeId == chargeId).First();
        }

        public bool SavePayment(Payment payment)
        {
            try
            {
                _context.Payments.Add(payment);
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdatePayment(Payment payment)
        {
            try
            {
                _context.Entry(payment).State = EntityState.Modified;
                return _context.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool DeletePayment(int id)
        {
            try
            {
                var payment = _context.Payments.Find(id);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                    return _context.SaveChanges() > 0;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
