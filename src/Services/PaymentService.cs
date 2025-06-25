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
        private readonly ICheckInnContext _context;

        public PaymentService(ICheckInnContext dbContext)
        {
            _context = dbContext;
        }

        public Payment GetPaymentByID(int id)
        {
            return _context.Payments.Find(id);
        }

        public IEnumerable<Payment> GetPaymentsByBookingID(int bookingID)
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
        public List<Payment> SearchPayments(string searchTerm, string paymentStatus, int? page, int? recordSize)
        {
            var payments = _context.Payments
                .Include(p => p.Booking)
                .Include(p => p.Booking.Accomodation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                payments = payments.Where(p =>
                    p.Booking.GuestName.Contains(searchTerm) ||
                    p.Booking.Email.Contains(searchTerm) ||
                    p.StripePaymentIntentId.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                payments = payments.Where(p => p.PaymentStatus == paymentStatus);
            }

            var skip = (page - 1) * recordSize;
            return payments
                .OrderByDescending(p => p.PaymentDate)
                .Skip((int)skip)
                .Take((int)recordSize)
                .ToList();
        }

        public int SearchPaymentsCount(string searchTerm, string paymentStatus)
        {
            var payments = _context.Payments.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                payments = payments.Where(p =>
                    p.Booking.GuestName.Contains(searchTerm) ||
                    p.Booking.Email.Contains(searchTerm) ||
                    p.StripePaymentIntentId.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(paymentStatus))
            {
                payments = payments.Where(p => p.PaymentStatus == paymentStatus);
            }

            return payments.Count();
        }

        public decimal GetTotalRevenue()
        {
            return _context.Payments
                .Where(p => p.PaymentStatus == "Completed")
                .Sum(p => p.Amount);
        }

        public decimal GetRevenueForDate(DateTime date)
        {
            return _context.Payments
                .Where(p => p.PaymentStatus == "Completed" &&
                           p.PaymentDate.Year == date.Year &&
                           p.PaymentDate.Month == date.Month &&
                           p.PaymentDate.Day == date.Day)
                .Sum(p => p.Amount);
        }

        public decimal GetRevenueForMonth(int month, int year)
        {
            return _context.Payments
                .Where(p => p.PaymentStatus == "Completed" &&
                           p.PaymentDate.Month == month &&
                           p.PaymentDate.Year == year)
                .Sum(p => p.Amount);
        }

        public Dictionary<string, int> GetPaymentStatusStatistics()
        {
            return _context.Payments
                .GroupBy(p => p.PaymentStatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Status, x => x.Count);
        }
    }
}
