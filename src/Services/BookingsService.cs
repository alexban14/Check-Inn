using Check_Inn.DAL;
using Check_Inn.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Check_Inn.Services
{
    public class BookingsService
    {
        private CheckInnContext context;

        public BookingsService()
        {
            context = new CheckInnContext();
        }
        
        public IEnumerable<Booking> GetAllAcomodationPackages()
        {
            return context.Bookings.ToList();
        }

        public IEnumerable<Booking> GetAllBookingsByAccomodationPackage(int accomodationID)
        {
            return context.Bookings.Where(x => x.AccomodationID == accomodationID).ToList();
        }

        public IEnumerable<Booking> SearchBooking(string searchTerm, int? AccomodationID, int? page, int? recordSize)
        {
            IEnumerable<Booking> bookings = context.Bookings.AsQueryable();

            if(!string.IsNullOrEmpty(searchTerm))
            {
                bookings = bookings.Where(a => a.GuestName.ToLower().Contains(searchTerm.ToLower()) );
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
            IEnumerable<Booking> bookings = context.Bookings.AsQueryable();

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
            using (CheckInnContext context = new CheckInnContext())
            { 
                return context.Bookings.Find(ID);
            }
        }

        public bool SaveBooking(Booking booking)
        {
            context.Bookings.Add(booking);

            return context.SaveChanges() > 0;
        }

        public bool UpdateBooking(Booking booking)
        {
            context.Entry(booking).State = System.Data.Entity.EntityState.Modified;

            return context.SaveChanges() > 0;
        }

        public bool DeleteBooking(Booking booking)
        {
            context.Entry(booking).State = System.Data.Entity.EntityState.Deleted;

            return context.SaveChanges() > 0;
        }

        public bool IsAccomodationAvailable(int accomodationID, DateTime fromDate, int duration)
        {
            DateTime toDate = fromDate.AddDays(duration - 1);

            var overlappingBookings = context.Bookings
                .Where(b => b.AccomodationID == accomodationID &&
                            !(fromDate > DbFunctions.AddDays(b.FromDate, b.Duration - 1) || toDate < b.FromDate))
                .ToList();

            return overlappingBookings.Count == 0;
        }
    }
}