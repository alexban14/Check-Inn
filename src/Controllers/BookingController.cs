using Check_Inn.Services;
using Check_Inn.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Check_Inn.Controllers
{
    public class BookingController : Controller
    {
        private BookingsService _bookingService;

        public BookingController(BookingsService bookingsService)
        {
            _bookingService = bookingsService;
        }

        // GET: Book
        public ActionResult Index(int? page = 1, int? recordSize = 10)
        {
            // Get current user's email from Identity
            string userEmail = User.Identity.GetUserName(); // In ASP.NET Identity, username is typically the email
            
            // Create a view model to hold the booking data
            var bookings = _bookingService.GetBookingsByUserEmail(userEmail, page, recordSize);
            var totalBookings = _bookingService.GetBookingCountByUserEmail(userEmail);
            
            var model = new BookingsListViewModel
            {
                Bookings = bookings,
                TotalCount = totalBookings,
                CurrentPage = page.Value,
                RecordSize = recordSize.Value
            };
            
            return View(model);
        }
        
        public ActionResult Details(int id)
        {
            var booking = _bookingService.GetBookingByID(id);
            
            // Check if booking exists and belongs to current user
            if (booking == null || booking.Email != User.Identity.GetUserName())
            {
                return HttpNotFound();
            }
            
            return View(booking);
        }
    }
}