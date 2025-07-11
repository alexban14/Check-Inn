﻿using Check_Inn.DAL;
using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Check_Inn.Services
{
    public class BookingsService
    {
        private readonly ICheckInnContext _context;
        private EmailService _emailService;

        public BookingsService(
            ICheckInnContext context,
            EmailService emailService
        )
        {
            _context = context;
            _emailService = emailService;
        }
        
        public IEnumerable<Booking> GetAllAcomodationPackages()
        {
            return _context.Bookings.ToList();
        }

        public IEnumerable<Booking> GetAllBookingsByAccomodationPackage(int accomodationID)
        {
            return _context.Bookings.Where(x => x.AccomodationID == accomodationID).ToList();
        }

        public IEnumerable<Booking> GetBookingsByUserEmail(string email, int? page = 1, int? recordSize = 10)
        {
            var bookings = _context.Bookings.Where(b => b.Email == email);

            int skip = (page.Value - 1) * recordSize.Value;
            return bookings
                .OrderByDescending(x => x.FromDate)
                .Skip(skip)
                .Take(recordSize.Value)
                .Include(b => b.Accomodation)
                .ToList();
        }

        public int GetBookingCountByUserEmail(string email)
        {
            return _context.Bookings.Count(b => b.Email == email);
        }

        public IEnumerable<Booking> SearchBooking(string searchTerm, int? AccomodationID, int? page, int? recordSize)
        {
            IEnumerable<Booking> bookings = _context.Bookings.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                bookings = bookings.Where(a => a.GuestName.ToLower().Contains(searchTerm.ToLower()));
            }

            if (AccomodationID.HasValue && AccomodationID > 0)
            {
                bookings = bookings.Where(a => a.AccomodationID == AccomodationID.Value);
            }

            var skip = (page - 1) * recordSize;

            return bookings
                .OrderBy(x => x.FromDate)
                .Skip((int)skip)
                .Take((int)recordSize)
                .ToList();
        }

        public int SearchBookingCount(string searchTerm, int? AccomodationID)
        {
            IEnumerable<Booking> bookings = _context.Bookings.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                bookings = bookings.Where(a => a.GuestName.ToLower().Contains(searchTerm.ToLower()) );
            }

            if (AccomodationID.HasValue && AccomodationID > 0)
            {
                bookings = bookings.Where(a => a.AccomodationID == AccomodationID.Value);
            }

            return bookings.Count();
        }

        public Booking GetBookingByID(int ID)
        {
            return _context.Bookings.Find(ID);
        }

        public Booking GetBookingWithPaymentsByID(int ID)
        {
            return _context.Bookings
                .Include(b => b.Accomodation)
                .Include(b => b.Accomodation.AccomodationPackage)
                .FirstOrDefault(b => b.ID == ID);
        }

        public bool SaveBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            bool saved = _context.SaveChanges() > 0;

            if (saved)
            {
                SendBookingConfirmationEmail(booking);
            }

            return saved;
        }

        public bool UpdateBooking(Booking booking)
        {
            _context.Entry(booking).State = System.Data.Entity.EntityState.Modified;

            return _context.SaveChanges() > 0;
        }

        public bool DeleteBooking(Booking booking)
        {
            _context.Entry(booking).State = System.Data.Entity.EntityState.Deleted;

            return _context.SaveChanges() > 0;
        }

        public bool IsAccomodationAvailable(int accomodationID, DateTime fromDate, int duration)
        {
            DateTime toDate = fromDate.AddDays(duration - 1);

            var overlappingBookings = _context.Bookings
                .Where(b => b.AccomodationID == accomodationID)
                .ToList()
                .Where(b =>
                    fromDate <= b.FromDate.AddDays(b.Duration - 1) &&
                    toDate >= b.FromDate
                );

            return !overlappingBookings.Any();

            /*
            var overlappingBookings = _context.Bookings
                .Where(b => b.AccomodationID == accomodationID)
                .ToList() // Fetch all bookings for the given accommodation in memory
                .Where(b => !(fromDate > b.FromDate.AddDays(b.Duration - 1) || toDate < b.FromDate))
                .ToList();

            bool bookingsOverlap = overlappingBookings.Count < 1;

            return bookingsOverlap;
            */

            /*
                .Where(b => b.AccomodationID == accomodationID &&
                            !(fromDate > DbFunctions.AddDays(b.FromDate, b.Duration - 1) || toDate < b.FromDate))
                .ToList();
            */

        }

        public bool HasPendingPayment(int bookingId)
        {
            return _context.Payments
                .Any(p => p.BookingID == bookingId && p.PaymentStatus == "Pending");
        }
        
        public bool HasCompletedPayment(int bookingId)
        {
            return _context.Payments
                .Any(p => p.BookingID == bookingId && p.PaymentStatus == "Completed");
        }

        private bool SendBookingConfirmationEmail(Booking booking)
        {
            try
            {
                var accomodation = _context.Accomodations
                    .Include(a => a.AccomodationPackage)
                    .FirstOrDefault(a => a.ID == booking.AccomodationID);

                if (accomodation == null || accomodation.AccomodationPackage == null)
                {
                    return false;
                }

                return _emailService.SendBookingConfirmation(
                    booking, 
                    accomodation, 
                    accomodation.AccomodationPackage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to send booking confirmation: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> SendBookingConfirmationEmailAsync(Booking booking)
        {
            try
            {
                var accomodation = await _context.Accomodations
                    .Include(a => a.AccomodationPackage)
                    .FirstOrDefaultAsync(a => a.ID == booking.AccomodationID);

                if (accomodation == null || accomodation.AccomodationPackage == null)
                {
                    return false;
                }

                return await _emailService.SendBookingConfirmationAsync(
                    booking, 
                    accomodation, 
                    accomodation.AccomodationPackage);
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }
    }
}